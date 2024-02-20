using Microsoft.Extensions.Configuration;
using Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;
using System.Diagnostics;
[assembly: InternalsVisibleTo("Test")]
namespace Engine // Note: actual namespace depends on the project name.
{
    internal class Controller
    {

        static Platform GUIPlatform = PlatformUtilities.Generate32x20();
        static string TAG_TICK = "tick";
        static string TAG_CORRECTION = "correction";
        static string TAG_INIT = "init";
        static string TAG_COMPLETE = "complete";


        internal void Execute(string PathToRecoveryResult, string PathToBasmResult, List<Tuple<List<int>, List<int>>> basmInstructions, JArray expectedPositions, SerialManager manager)
        {
            manager.OpenPort();
            // TIP: At the very beginning, we need to send some commands to clean the entire board.

            TurnOnHighVoltage();
            SendToDMF(manager, PathToBasmResult,TAG_INIT);

            ClearAllElectrodes();
            SendToDMF(manager, PathToBasmResult, TAG_INIT);

            // ExecutePreTest(manager);

            ExecuteCorrection(basmInstructions, expectedPositions, PathToRecoveryResult, PathToBasmResult, manager);

            ClearAllElectrodes();
            SendToDMF(manager, PathToBasmResult, TAG_COMPLETE);
        }

        internal List<Tuple<List<int>, List<int>>> InitBasmInstructions(string pathToBasmFile)
        {
            var basmPerTick = new List<Tuple<List<int>, List<int>>>();
            List<int> clearElectrodes = null;
            List<int> setElectrodes = null;

            foreach (var line in File.ReadLines(pathToBasmFile))
            {
                if (line.Trim().Equals("TICK;"))
                {
                    if (setElectrodes != null || clearElectrodes != null)
                    {
                        basmPerTick.Add(new Tuple<List<int>, List<int>>(clearElectrodes, setElectrodes));
                    }
                    setElectrodes = new List<int>();
                    clearElectrodes = new List<int>();
                }
                else if (line.StartsWith("SETELI"))
                {
                    var value = int.Parse(line.Split(' ')[1].Trim(';'));
                    setElectrodes.Add(value);
                }
                else if (line.StartsWith("CLRELI"))
                {
                    var value = int.Parse(line.Split(' ')[1].Trim(';'));
                    clearElectrodes.Add(value);
                }
            }

            // Add the last group if it exists
            if (setElectrodes != null || clearElectrodes != null)
            {
                basmPerTick.Add(new Tuple<List<int>, List<int>>(clearElectrodes, setElectrodes));
            }

            return basmPerTick;
        }


        internal JArray InitExpectedStatus(string pathToExp)
        {
            // Parse router.json file.
            string routerJsonText = File.ReadAllText(@$"{pathToExp}");
            JArray routerJsonArray = JArray.Parse(routerJsonText);
            return routerJsonArray;
        }

        void ExecuteCorrection(List<Tuple<List<int>, List<int>>> basmPerTick, JArray expectedPositions, string pathToRecoveryResult, string PathToBasmResult, SerialManager manager)
        {
            MqttClient client = new MqttClient("localhost");
            client.Subscribe(MqttTopic.YOLO_ACTUAL);

            // Initialize a corrector
            Corrector corrector = new Corrector();

            // counter for tick
            int tick = 0;
            // Execute first 3 ticks, i.e. the first movement
            for (int j = 0; j < 3; j++)
            {
                PrintContentOfSetAndClearList(basmPerTick[tick].Item1, basmPerTick[tick].Item2);
                UpdateElectrodes(basmPerTick[tick].Item1, basmPerTick[tick].Item2);
                SendToDMF(manager, PathToBasmResult, TAG_TICK + tick);
                tick++;
            } 
            
            DateTime recordTime = client.previousUpdateTime;
            // i is the index of expected state
            for (int i = 0; i < expectedPositions.Count; i++)
            {
                string expectedStates = JsonConvert.SerializeObject(expectedPositions[i]["exp"], Formatting.None).ToString();
                Console.WriteLine("step " + i + ":" + expectedStates);
                List<Dictionary<string, HashSet<int>>> electrodesForRecovery;
                while (client.previousActualState == "" || client.previousActualState == "[]"  || DateTime.Compare(recordTime, client.previousUpdateTime)==0 )
                {
                    Console.WriteLine("Waiting for time update:"+ client.previousActualState);
                    Thread.Sleep(100);
                }
                // Correct by given expected states and actual states
                recordTime = client.previousUpdateTime;
                electrodesForRecovery = corrector.Run(expectedStates, client.previousActualState, pathToRecoveryResult,true);

                // If correction result is an empty list (i.e. Actual states match expected states), then execute next movement.
                if (IsEmpty(electrodesForRecovery))
                {
                    Console.WriteLine("****************Is empty!****************");
                    do
                    {
                        PrintContentOfSetAndClearList(basmPerTick[tick].Item1, basmPerTick[tick].Item2);
                        UpdateElectrodes(basmPerTick[tick].Item1, basmPerTick[tick].Item2);
                        SendToDMF(manager, PathToBasmResult, TAG_TICK + tick);
                        tick++;  
                    }
                    while (tick % 2 == 0 && tick < basmPerTick.Count() - 1);
                    //Thread.Sleep(500);
                }
                else
                {
                    int correctionCounter = 0;
                    while (true)
                    {
                        Console.WriteLine("****************NOT empty!****************");

                        HashSet<int> toClear = new HashSet<int>();
                        HashSet<int> toSet = new HashSet<int>();

                        // 1. the first step to correct: set tails
                        foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                        {
                            toSet.UnionWith(elsPerDroplet["tail"]);
                        }
                        PrintContentOfSetAndClearList(toClear.ToList(), toSet.ToList());
                        UpdateElectrodes(toClear.ToList(), toSet.ToList());
                        SendToDMF(manager, PathToBasmResult, $"{TAG_TICK}{tick}:{TAG_CORRECTION}{correctionCounter}:SetTail");

                        toClear.Clear();
                        toSet.Clear();
                        // 2. the second step to correct: clear head
                        foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                        {
                            toClear.UnionWith(elsPerDroplet["head"]);
                        }
                        PrintContentOfSetAndClearList(toClear.ToList(), toSet.ToList());
                        UpdateElectrodes(toClear.ToList(), toSet.ToList());
                        SendToDMF(manager, PathToBasmResult, $"{TAG_TICK}{tick}:{TAG_CORRECTION}{correctionCounter}:ClearHead");

                        toClear.Clear();
                        toSet.Clear();
                        // 3. the third step to correct: select head
                        foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                        {
                            toSet.UnionWith(elsPerDroplet["head"]);
                        }
                        PrintContentOfSetAndClearList(toClear.ToList(), toSet.ToList());
                        UpdateElectrodes(toClear.ToList(), toSet.ToList());
                        SendToDMF(manager, PathToBasmResult, $"{TAG_TICK}{tick}:{TAG_CORRECTION}{correctionCounter}:SetHead");

                        toClear.Clear();
                        toSet.Clear();
                        // 4. the forth step: clear tail
                        foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                        {
                            toClear.UnionWith(elsPerDroplet["tail"]);
                        }
                        PrintContentOfSetAndClearList(toClear.ToList(), toSet.ToList());
                        UpdateElectrodes(toClear.ToList(), toSet.ToList());
                        SendToDMF(manager, PathToBasmResult, $"{TAG_TICK}{tick}:{TAG_CORRECTION}{correctionCounter}:ClearTail");

                        // wait for execution
                        while (client.previousActualState == "" || DateTime.Compare(recordTime, client.previousUpdateTime) == 0)
                        {
                            Thread.Sleep(500);
                            Console.WriteLine("Waiting for yolo update");
                        }
                        // Check if correnction is success. If the electrodesForRecovery is empty, the correnction is success.
                        recordTime = client.previousUpdateTime;
                        electrodesForRecovery = corrector.Run(expectedStates, client.previousActualState, pathToRecoveryResult, true);
                        if (IsEmpty(electrodesForRecovery))
                        {
                            Console.WriteLine("****************Break****************");
                            break;
                        }
                        correctionCounter++;
                        Thread.Sleep(500);
                    }
                }
                // Wait for YOLO and router to publish. TODO: Is it needed?
                // Thread.Sleep(1000);
                corrector.PrintCalculatedTime();
            }
        }


        void UpdateElectrodes(List<int> electrodesToClear, List<int> electrodesToSet)
        {
            const int maxElectrodeInCommand = 10;

            List<int> electrodesDriver0 = new List<int>();
            List<int> electrodesDriver1 = new List<int>();
            string command = "";

            electrodesDriver0.Clear();
            electrodesDriver1.Clear();
            foreach (int electrode in electrodesToSet)
            {
                if (GUIPlatform.electrodes[electrode - 1].driverID == 0)
                {
                    electrodesDriver0.Add(electrode);
                }
                else if (GUIPlatform.electrodes[electrode - 1].driverID == 1)
                {
                    electrodesDriver1.Add(electrode);
                }
                else
                {
                    throw new Exception("Driver ID not valid.");
                }
            }
            command = "";
            for (int j = 0; j < electrodesDriver0.Count; j = j + maxElectrodeInCommand)
            {
                command = Commands.SET_ELEC + " 0";
                for (int i = j; (i < j + maxElectrodeInCommand && i < electrodesDriver0.Count); i++)
                {
                    command = command + " " + GUIPlatform.electrodes[electrodesDriver0[i] - 1].electrodeID;
                }
                GUIPlatform.commands.Add(command);
                command = "";
            }
            command = "";
            for (int j = 0; j < electrodesDriver1.Count; j = j + maxElectrodeInCommand)
            {
                command = Commands.SET_ELEC + " 1";
                for (int i = j; (i < j + maxElectrodeInCommand && i < electrodesDriver1.Count); i++)
                {
                    command = command + " " + GUIPlatform.electrodes[electrodesDriver1[i] - 1].electrodeID;
                }
                GUIPlatform.commands.Add(command);
                command = "";
            }

            electrodesDriver0.Clear();
            electrodesDriver1.Clear();
            foreach (int electrode in electrodesToClear)
            {
                if (GUIPlatform.electrodes[electrode - 1].driverID == 0)
                {
                    electrodesDriver0.Add(electrode);
                }
                else if (GUIPlatform.electrodes[electrode - 1].driverID == 1)
                {
                    electrodesDriver1.Add(electrode);
                }
                else
                {
                    throw new Exception("Driver ID not valid.");
                }
            }
            command = "";
            for (int j = 0; j < electrodesDriver0.Count; j = j + maxElectrodeInCommand)
            {
                command = Commands.CLR_ELEC + " 0";
                for (int i = j; (i < j + maxElectrodeInCommand && i < electrodesDriver0.Count); i++)
                {
                    command = command + " " + GUIPlatform.electrodes[electrodesDriver0[i] - 1].electrodeID;
                }
                GUIPlatform.commands.Add(command);
                command = "";
            }
            command = "";
            for (int j = 0; j < electrodesDriver1.Count; j = j + maxElectrodeInCommand)
            {
                command = Commands.CLR_ELEC + " 1";
                for (int i = j; (i < j + maxElectrodeInCommand && i < electrodesDriver1.Count); i++)
                {
                    command = command + " " + GUIPlatform.electrodes[electrodesDriver1[i] - 1].electrodeID;
                }
                GUIPlatform.commands.Add(command);
                command = "";
            }
            // Clear electrodes
            if (electrodesToClear.Count != 0)
            {
                string answer = "";
                answer += "Electrodes to clear: ";
                foreach (int electrode in electrodesToClear)
                {
                    answer += '\n' + electrode.ToString();
                    GUIPlatform.electrodes[electrode - 1].status = false;
                }
            }
            // Set electrodes
            if (electrodesToSet.Count != 0)
            {
                string answer = "";
                answer += "Electrodes to set: ";
                foreach (int electrode in electrodesToSet)
                {
                    answer += '\n' + electrode.ToString();
                    GUIPlatform.electrodes[electrode - 1].status = true;
                }
            }
        }


        void ClearAllElectrodes()
        {
            GUIPlatform.commands.Add(Commands.CLR_ALL + " 0");
            GUIPlatform.commands.Add(Commands.CLR_ALL + " 1");
        }


        void SendToDMF(SerialManager manager, string PathToBasmResult , string tag)
        {
            Logger.LogSendToDMF("Sending to DMF");
            foreach (var command in GUIPlatform.commands)
            {
                Logger.LogSendToDMF(command);
                File.AppendAllText(PathToBasmResult, $"[{tag}]:{command}\n");
                string serialCommand = command + Commands.TERMINATOR;
                manager.Write(serialCommand);
                Thread.Sleep(200);
            }
            Console.WriteLine();
            GUIPlatform.commands.Clear();
            Logger.LogSendToDMF("Delay...");
            Thread.Sleep(200);
            Logger.LogSendToDMF("Done.");
        }


        void TurnOnHighVoltage()
        {
            GUIPlatform.commands.Add(Commands.HV_SET + " 1 280");
            GUIPlatform.commands.Add(Commands.HV_ON_OFF + " 1 1");
        }


        void TurnOffHighVoltage()
        {
            GUIPlatform.commands.Add(Commands.HV_ON_OFF + " 1 0");
        }


        static bool IsEmpty(List<Dictionary<string, HashSet<int>>> list)
        {
            // Check if the list itself is empty
            if (list.Count == 0)
            {
                return true;
            }

            // Iterate through each dictionary in the list
            foreach (var dictionary in list)
            {
                // If any dictionary is empty, we continue to the next dictionary
                if (dictionary.Count == 0)
                {
                    continue;
                }

                // Check each HashSet in the dictionary
                foreach (var hashSet in dictionary.Values)
                {
                    // If any HashSet is not empty, the entire structure is not empty
                    if (hashSet.Count != 0)
                    {
                        return false;
                    }
                }
            }

            // If all HashSets in all dictionaries are empty, then the structure is empty
            return true;
        }


        void PrintContentOfSetAndClearList(List<int> clearElec, List<int> setElec)
        {
            // Print setElectrodes and clearElectrodes of currrent TiCK
            Console.WriteLine("TICK;");

            // Print setElectrodes list
            Console.Write("  clearElectrodes: [");
            Console.Write(string.Join(", ", clearElec));
            Console.WriteLine("]");

            // Print clearElectrodes list
            Console.Write("  setElectrodes: [");
            Console.Write(string.Join(", ", setElec));
            Console.WriteLine("]");

            Console.WriteLine(); // Adding a blank line for better readability
        }


        void ExecutePreTest(SerialManager manager)
        {
            /*Console.WriteLine("Start pre test******************************");
            UpdateElectrodes(new List<int>() { }, new List<int>() { 545 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            UpdateElectrodes(new List<int>() { }, new List<int>() { 513 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step1******************************");
            UpdateElectrodes(new List<int>() { 545 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step2******************************");
            UpdateElectrodes(new List<int>() { }, new List<int>() { 481 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step3******************************");
            UpdateElectrodes(new List<int>() { 513 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step4******************************");
            UpdateElectrodes(new List<int>() { }, new List<int>() { 449 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step5******************************");
            UpdateElectrodes(new List<int>() { 481 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step7******************************");
            UpdateElectrodes(new List<int>() { }, new List<int>() { 417 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            UpdateElectrodes(new List<int>() { 449 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step8******************************");
            UpdateElectrodes(new List<int>() { }, new List<int>() { 385 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            UpdateElectrodes(new List<int>() { 417 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step9******************************");
            UpdateElectrodes(new List<int>() { }, new List<int>() { 353 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("done******************************");
            UpdateElectrodes(new List<int>()
            {
            }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(500);*/
        }
    }
}