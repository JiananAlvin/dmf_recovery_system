using Microsoft.Extensions.Configuration;
using Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Threading;

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

            List<List<string>> basmInstructions = InitBasmInstructions(config["bio-assembly-src"]!);
            JArray expectedPositions = InitExpectedStatus(config["expected-position"]!);
            ExecuteCorrection(basmInstructions, expectedPositions, config["path-to-result"]!);
        }

        static List<List<string>> InitBasmInstructions(string pathToBasmFile)
        {
            // read all lines from file
            string[] lines = File.ReadAllLines(pathToBasmFile);

            List<List<string>> groupedBasmInstruction = new List<List<string>>();
            List<string> currentGroup = null;

            foreach (string line in lines)
            {
                if (line.Trim() == "TICK;")
                {
                    // When meet 'TICK', create a new group
                    if (currentGroup != null)
                    {
                        groupedBasmInstruction.Add(currentGroup);
                    }
                    currentGroup = new List<string>();
                }
                else if (currentGroup != null)
                {
                    currentGroup.Add(line);
                }
            }

            // Add the last group
            if (currentGroup != null && currentGroup.Count > 0)
            {
                groupedBasmInstruction.Add(currentGroup);
            }
            return groupedBasmInstruction;
        }

        public static JArray InitExpectedStatus(string pathToExp)
        {
            // Parse router.json file.
            string routerJsonText = File.ReadAllText(@$"{pathToExp}");
            JArray routerJsonArray = JArray.Parse(routerJsonText);
            return routerJsonArray;
        }

        static void ExecuteCorrection(List<List<string>> basmInstructions, JArray expectedPositions, string pathToResult)
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
                        do
                        {
                            Console.WriteLine("Group:");
                            Console.WriteLine(counter);
                            foreach (var line in basmInstructions[counter])
                            {
                                Console.WriteLine(line);
                            }
                            Console.WriteLine();
                            counter++;
                        }
                        while (counter % 4 != 0);
                    }
                    else
                    {
                        //todo: parse    //head: [ 326 327 328 ]   //tail: [ 422 423 424 ] to basm
                        Console.WriteLine("*****TODO: parse head: [ 326 327 328 ] tail: [ 422 423 424 ] to basm");
                    }

                    // Wait for YOLO and router to publish.
                    Thread.Sleep(1000);
                } while (electrodesForRecovery.Count != 0);
            }
        }
    }
}