using System.Diagnostics;
using Engine;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace UnitTest
{
    public class TestUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PythonCommand"> Define the python command or the full path to python.exe if not in PATH. It could be 'py' or 'python', which depends on your system. </param>
        /// <param name="PathToScript"></param>
        /// <param name="PathToWeight">The path to the weight</param>
        /// <param name="PathToSourceRoot">The root fold to the test cases</param>
        /// <param name="CaseName">Case0, Case1, Case2 ...</param>
        public static int RunYoloV5InPython(string PythonCommand, string PathToScript, string PathToWeight, string PathToSourceRoot, string CaseName)
        {
            // wait for Controller to run first
            Thread.Sleep(5000);

            string arguments = $"--weights {PathToWeight} --source {PathToSourceRoot}\\{CaseName} --require-preprocess";

            // Create process start info
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = PythonCommand,
                Arguments = $"{PathToScript} {arguments}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            // Start the process
            using (Process process = Process.Start(startInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }

                using (StreamReader reader = process.StandardError)
                {
                    string error = reader.ReadToEnd();
                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Error: " + error);
                    }
                }
                process.WaitForExit();
            }
            return 0;
        }

        public static async void PublishActStatesToMqtt(string file)
        {
            int n = 5;
            int timeInterval = 200;

            // Configure MQTT client options
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883) 
                .Build();

            // Create a new MQTT client
            var mqttFactory = new MqttFactory();
            var mqttClient = mqttFactory.CreateMqttClient();

            // Connect to MQTT broker
            await mqttClient.ConnectAsync(mqttClientOptions);

            // Read the file line by line
            using (var reader = new StreamReader(file))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    // Create the message to publish
                    var message = new MqttApplicationMessageBuilder()
                        .WithTopic(MqttTopic.YOLO_ACTUAL)
                        .WithPayload(line)
                        .WithExactlyOnceQoS() 
                        .Build();

                    // Publish the same message every 200 ms and 5 times.
                    for (int i  = 0; i < n; i++)
                    {
                        // Publish the message
                        await mqttClient.PublishAsync(message);
                        Thread.Sleep(timeInterval);
                    }
                }
            }

            // Disconnect from MQTT broker
            await mqttClient.DisconnectAsync();
        }
    }
}