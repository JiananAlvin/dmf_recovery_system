using Microsoft.Extensions.Configuration;
using Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Threading;
using System.Xml.Linq;
using NetTopologySuite.Index.HPRtree;
using System.Linq;

namespace Engine // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        /********************************
         args:
        --platform-setting : The json file of the description of the platform
        --expected-position : The file describing the expected positions
        --bio-assembly-src : The bio assembly source file (protocal)
        --steps: The steps 
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
            //SelectSerialPort();
            // Clear the whole DMF chip
            SerialManager manager = new SerialManager(config["serial-port"]!, 1000);
            // TIP: At the very beginning, we need to send some commands to clean the entire board.
            ClearAllElectrodes();
            SendToDMF(manager);


            ExecuteCorrection(basmInstructions, expectedPositions, config["path-to-result"]!, manager);
        }

        public static List<Tuple<List<int>, List<int>>> InitBasmInstructions(string pathToBasmFile)
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

        public static JArray InitExpectedStatus(string pathToExp)
        {
            // Parse router.json file.
            string routerJsonText = File.ReadAllText(@$"{pathToExp}");
            JArray routerJsonArray = JArray.Parse(routerJsonText);
            return routerJsonArray;
        }

        //static void Parse
        static void ExecuteCorrection(List<Tuple<List<int>, List<int>>> basmPerTick, JArray expectedPositions, string pathToResult, SerialManager manager)
        {
            MqttClient client = new MqttClient("localhost");
            client.Subscribe(MqttTopic.YOLO_ACTUAL);

            // Initialize a corrector
            Corrector corrector = new Corrector();
            // output 4 group per second
            int counter = 0;

            for (int i = 0; i < expectedPositions.Count; i++)
            {
                Console.WriteLine($"Exec is running for round {i}");
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
                    if (electrodesForRecovery.Count == 0)
                    {
                        // print basm 
                        // todo: send these instructions to simulator
                        Console.WriteLine("-------------------------------------------");
                        do
                        {
                            // Print setElectrodes and clearElectrodes of currrent TiCK
                            Console.WriteLine("TICK;");

                            // Print setElectrodes list
                            Console.Write("  setElectrodes: [");
                            Console.Write(string.Join(", ", basmPerTick[counter].Item1));
                            Console.WriteLine("]");

                            // Print clearElectrodes list
                            Console.Write("  clearElectrodes: [");
                            Console.Write(string.Join(", ", basmPerTick[counter].Item2));
                            Console.WriteLine("]");

                            Console.WriteLine(); // Adding a blank line for better readability

                            UpdateElectrodes(basmPerTick[counter].Item1, basmPerTick[counter].Item2);
                            SendToDMF(manager);

                            counter++;
                        }
                        while (counter % 4 != 0);
                        Console.WriteLine("------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine("******************************************");
                        Console.WriteLine("TICK;");
                        HashSet<int> toClear = new HashSet<int>();
                        HashSet<int> toAct = new HashSet<int>();
                        foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                        {
                            foreach (int element in elsPerDroplet["tail"])
                            {
                                Console.WriteLine($"SETELI {element};");

                            }
                            toAct.UnionWith(elsPerDroplet["tail"]);
                        }
                        UpdateElectrodes(toClear.ToList(), toAct.ToList());
                        SendToDMF(manager);

                        toClear.Clear();
                        toAct.Clear();
                        Console.WriteLine("TICK;");
                        foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                        {
                            foreach (int element in elsPerDroplet["head"])
                            {
                                Console.WriteLine($"CLRELI {element};");
                            }
                            toClear.UnionWith(elsPerDroplet["head"]);
                        }
                        UpdateElectrodes(toClear.ToList(), toAct.ToList());
                        SendToDMF(manager);


                        toClear.Clear();
                        toAct.Clear();
                        Console.WriteLine("TICK;");
                        foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                        {
                            foreach (int element in elsPerDroplet["head"])
                            {
                                Console.WriteLine($"SETELI {element};");
                            }
                            toAct.UnionWith(elsPerDroplet["head"]);
                        }
                        UpdateElectrodes(toClear.ToList(), toAct.ToList());
                        SendToDMF(manager);

                        toClear.Clear();
                        toAct.Clear();
                        Console.WriteLine("TICK;");
                        foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
                        {
                            foreach (int element in elsPerDroplet["tail"])
                            {
                                Console.WriteLine($"CLRELI {element};");
                            }
                            toClear.UnionWith(elsPerDroplet["tail"]);
                        }
                        UpdateElectrodes(toClear.ToList(), toAct.ToList());
                        SendToDMF(manager);

                        Console.WriteLine("******************************************");
                        // wait for execution correction
                        Thread.Sleep(5000);
                    }

                    // Wait for YOLO and router to publish.
                    Thread.Sleep(1000);
                } while (electrodesForRecovery.Count != 0);
            }
        }

        static Platform GUIPlatform = PlatformUtilities.Generate32x20();

        public static void UpdateElectrodes(List<int> electrodesToClear, List<int> electrodesToSet)
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

            //Clear electrodes
            if (electrodesToClear.Count != 0)
            {
                string answer = "";
                answer += "Electrodes to clear: ";
                foreach (int electrode in electrodesToClear)
                {
                    answer += '\n' + electrode.ToString();
                    GUIPlatform.electrodes[electrode - 1].status = false;
                }
                //logger.Debug(answer);
            }
            //Set electrodes
            if (electrodesToSet.Count != 0)
            {
                string answer = "";
                answer += "Electrodes to set: ";
                foreach (int electrode in electrodesToSet)
                {
                    answer += '\n' + electrode.ToString();
                    GUIPlatform.electrodes[electrode - 1].status = true;
                }
                //logger.Debug(answer);
            }
        }

        public static void ClearAllElectrodes()
        {
            GUIPlatform.commands.Add(Commands.CLR_ALL + " 0");
            GUIPlatform.commands.Add(Commands.CLR_ALL + " 1");
            foreach (var electrode in GUIPlatform.electrodes)
            {
                electrode.status = false;
            }
            //logger.Debug("Clearing all electrodes.");
        }

        public static void SendToDMF(SerialManager manager)
        {
            Console.WriteLine("Sending to DMF (PLACEHOLDER)");
            manager.OpenPort();
            foreach (var command in GUIPlatform.commands)
            {
                Console.WriteLine(command);
                manager.Write(command);

            }
            GUIPlatform.commands.Clear();
        }

        /*        public static void SelectSerialPort()
                {
                    Console.WriteLine("Available serial ports:");

                     var serialPorts = SerialManager.ListSerialPorts();
                    foreach (var port in serialPorts)
                    {
                        Console.WriteLine(port);
                    }
                    Console.Write("Enter port: ");
                    string inputString = Console.ReadLine();
                    Console.WriteLine("You entered: " + inputString);
                }*/
    }
}