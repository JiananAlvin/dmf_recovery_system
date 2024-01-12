using Microsoft.Extensions.Configuration;
using Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Engine // Note: actual namespace depends on the project name.
{
    internal class Program
    {

        static Platform GUIPlatform = PlatformUtilities.Generate32x20();

        /********************************
         args:
        --platform-setting : The json file of the description of the platform
        --expected-position : The file describing the expected positions
        --bio-assembly-src : The bio assembly source file (protocal)
        --steps : The steps 
        --serial-port : COM4
        ********************************/
        static void Main(string[] args)
        {
            // read args
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var config = builder.Build();

            File.Create(config["path-to-result"]!).Close();

            List<Tuple<List<int>, List<int>>> basmInstructions = InitBasmInstructions(config["bio-assembly-src"]!);
            JArray expectedPositions = InitExpectedStatus(config["expected-position"]!);

            // Read serial port
            // SelectSerialPort();
            // Clear the whole DMF chip
            SerialManager manager = new SerialManager(config["serial-port"]!, 115200);
            manager.OpenPort();
            // TIP: At the very beginning, we need to send some commands to clean the entire board.

            TurnOnHighVoltage();
            SendToDMF(manager);

            ClearAllElectrodes();
            SendToDMF(manager);

            ExecuteCorrection(basmInstructions, expectedPositions, config["path-to-result"]!, manager);
        }

        static List<Tuple<List<int>, List<int>>> InitBasmInstructions(string pathToBasmFile)
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

        static JArray InitExpectedStatus(string pathToExp)
        {
            // Parse router.json file.
            string routerJsonText = File.ReadAllText(@$"{pathToExp}");
            JArray routerJsonArray = JArray.Parse(routerJsonText);
            return routerJsonArray;
        }

        // static void Parse
        static void ExecuteCorrection(List<Tuple<List<int>, List<int>>> basmPerTick, JArray expectedPositions, string pathToResult, SerialManager manager)
        {
            ExecutePreTest(manager);
            /*
                        MqttClient client = new MqttClient("localhost");
                        client.Subscribe(MqttTopic.YOLO_ACTUAL);

                        // Initialize a corrector
                        Corrector corrector = new Corrector();
                        // output 4 group per second
                        int counter = 0;

                        for (int i = 0; i < expectedPositions.Count; i++)
                        {
                            // Console.WriteLine($"Exec is running for round {i}");
                            // todo: check the logic here 
                            DateTime recordTime = client.previousUpdateTime.AddMinutes(1);
                            string expectedStates = JsonConvert.SerializeObject(expectedPositions[i]["exp"], Formatting.None).ToString();

                            List<Dictionary<string, HashSet<int>>> electrodesForRecovery;
                            do
                            {
                                while (client.previousActualState == "" || recordTime == client.previousUpdateTime)
                                {
                                    Thread.Sleep(100);
                                }
                                // Correct by given expected states and actual states
                                recordTime = client.previousUpdateTime;
                                electrodesForRecovery = corrector.Run(expectedStates, client.previousActualState, pathToResult);

                                // If correction result is an empty list (i.e. Actual states match expected states), then give "okay" to executor.
                                if (IsEmpty(electrodesForRecovery))
                                {
                                    // print basm 
                                    // todo: send these instructions to simulator
                                    do
                                    {
                                        // TODO !!!!!!!
                                        PrintContentOfSetAndClearList(basmPerTick[counter].Item1, basmPerTick[counter].Item2);
                                        UpdateElectrodes(basmPerTick[counter].Item1, basmPerTick[counter].Item2);
                                        SendToDMF(manager);
                                        Thread.Sleep(100);

                                        counter++;
                                    }
                                    while (counter % 4 != 0 && counter < basmPerTick.Count() - 1);
                                }
                                else
                                {
                                    HashSet<int> toClear = new HashSet<int>();
                                    HashSet<int> toSet = new HashSet<int>();

                                    // 1. the first step to correct: set tails
                                    foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                                    {
                                        toSet.UnionWith(elsPerDroplet["tail"]);
                                    }
                                    PrintContentOfSetAndClearList(toClear.ToList(), toSet.ToList());
                                    UpdateElectrodes(toClear.ToList(), toSet.ToList());
                                    SendToDMF(manager);

                                    toClear.Clear();
                                    toSet.Clear();
                                    // 2. the second step to correct: clear head
                                    foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                                    {
                                        toClear.UnionWith(elsPerDroplet["head"]);
                                    }
                                    PrintContentOfSetAndClearList(toClear.ToList(), toSet.ToList());
                                    UpdateElectrodes(toClear.ToList(), toSet.ToList());
                                    SendToDMF(manager);

                                    toClear.Clear();
                                    toSet.Clear();
                                    // 3. the third step to correct: select head
                                    foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                                    {
                                        toSet.UnionWith(elsPerDroplet["head"]);
                                    }
                                    PrintContentOfSetAndClearList(toClear.ToList(), toSet.ToList());
                                    UpdateElectrodes(toClear.ToList(), toSet.ToList());
                                    SendToDMF(manager);

                                    toClear.Clear();
                                    toSet.Clear();
                                    // 4. the forth step: clear tail
                                    foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                                    {
                                        toClear.UnionWith(elsPerDroplet["tail"]);
                                    }
                                    PrintContentOfSetAndClearList(toClear.ToList(), toSet.ToList());
                                    UpdateElectrodes(toClear.ToList(), toSet.ToList());
                                    SendToDMF(manager);

                                    // wait for execution correction
                                    Thread.Sleep(5000);
                                }

                                // Wait for YOLO and router to publish.
                                Thread.Sleep(1000);
                            } while (IsEmpty(electrodesForRecovery));
                        }

                        static void PrintContentOfSetAndClearList(List<int> clearElec, List<int> setElec)
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
                        }*/

        }

        static void ExecutePreTest(SerialManager manager)
        {
            Console.WriteLine("Start pre test******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { 554 });
            SendToDMF(manager);

            // Thread.Sleep(100);

            UpdateElectrodes(new List<int>() { }, new List<int>() { 522 });
            SendToDMF(manager);

            // Thread.Sleep(100);
            Console.WriteLine("step1******************************");


            UpdateElectrodes(new List<int>() { 554 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step2******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { 490 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step3******************************");

            UpdateElectrodes(new List<int>() { 522 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step4******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { 458 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step5******************************");

            UpdateElectrodes(new List<int>() { 490 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step7******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { 426 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            UpdateElectrodes(new List<int>() { 458 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step8******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { 394 });
            SendToDMF(manager);
            // Thread.Sleep(100);


            UpdateElectrodes(new List<int>() { 426 }, new List<int>() { });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step9******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { 362 });
            SendToDMF(manager);
            // Thread.Sleep(100);

            Console.WriteLine("step******************************");

            UpdateElectrodes(new List<int>() {  394 }, new List<int>() { });
            SendToDMF(manager);

            Console.WriteLine("step******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { 330 });
            SendToDMF(manager);
            Console.WriteLine("step******************************");

            UpdateElectrodes(new List<int>() { 362 }, new List<int>() { });
            SendToDMF(manager);
            Console.WriteLine("step******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { });
            SendToDMF(manager);
            Console.WriteLine("step******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { });
            SendToDMF(manager);

            Console.WriteLine("step******************************");

            UpdateElectrodes(new List<int>() { }, new List<int>() { 362 });
            SendToDMF(manager);
            Console.WriteLine("step******************************");

            UpdateElectrodes(new List<int>() { 330 }, new List<int>() { });
            SendToDMF(manager);

            UpdateElectrodes(new List<int>() { }, new List<int>() { 330 });
            SendToDMF(manager);
            Console.WriteLine("step******************************");

            UpdateElectrodes(new List<int>() { 362 }, new List<int>() { });
            SendToDMF(manager);
            Console.WriteLine("step******************************");

            Console.WriteLine("done******************************");

            UpdateElectrodes(new List<int>() { 330 }, new List<int>() { });
            SendToDMF(manager);
            /*                Thread.Sleep(500);

                            Console.WriteLine("Clear Board******************************");
                            ClearAllElectrodes();
                            SendToDMF(manager);*/
        }



        static void UpdateElectrodes(List<int> electrodesToClear, List<int> electrodesToSet)
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

        static void ClearAllElectrodes()
        {
            GUIPlatform.commands.Add(Commands.CLR_ALL + " 0");
            GUIPlatform.commands.Add(Commands.CLR_ALL + " 1");
            // logger.Debug("Clearing all electrodes.");
        }

        static void SendToDMF(SerialManager manager)
        {
            Logger.LogSendToDMF("Sending to DMF");
            //manager.OpenPort();
            Logger.LogSendToDMF("Content:");
            foreach (var command in GUIPlatform.commands)
            {
                Logger.LogSendToDMF(command);
                string serialCommand = command + Commands.TERMINATOR;
                manager.Write(serialCommand);
                Thread.Sleep(300);

            }
            Console.WriteLine();
            GUIPlatform.commands.Clear();
            Logger.LogSendToDMF("Delay...");
            Thread.Sleep(300);
            Logger.LogSendToDMF("Done.");
        }



        static void TurnOnHighVoltage()
        {
            GUIPlatform.commands.Add(Commands.HV_SET + " 1 280");
            GUIPlatform.commands.Add(Commands.HV_ON_OFF + " 1 1");
        }

        static void TurnOffHighVoltage()
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
    }
}