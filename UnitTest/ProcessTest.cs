using System.Diagnostics;
using Engine;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace UnitTest
{
    public class ProcessTest
    {
        static string PYTHON_COMMAND = "py";
        static string PATH_TO_WEIGHT = @"..\..\..\..\yolov5\runs\train\new-model-integrated-rgb\weights\best.pt";
        static string PATH_TO_PY_SCRIPT = @"..\..\..\..\yolov5\detect.py";
        static string PATH_TO_CASE_SOURCE_ROOT = @"..\..\..\..\Cases";

        static Platform GUIPlatform = PlatformUtilities.Generate32x20();

        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PythonCommand"> Define the python command or the full path to python.exe if not in PATH. It could be 'py' or 'python', which depends on your system. </param>
        /// <param name="PathToScript"></param>
        /// <param name="PathToWeight">The path to the weight</param>
        /// <param name="PathToSourceRoot">The root fold to the test cases</param>
        /// <param name="CaseName">Case0, Case1, Case2 ...</param>
        private static int RunYoloV5InPython(string PythonCommand, string PathToScript, string PathToWeight, string PathToSourceRoot, string CaseName)
        {
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

        // all droplets move successfully
        [Test]
        public void TestCorrect0()
        {

            // read args
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var config = builder.Build();
            Engine.Program.

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

            // ExecutePreTest(manager);

            ExecuteCorrection(basmInstructions, expectedPositions, config["path-to-result"]!, manager);

            ClearAllElectrodes();
            SendToDMF(manager);

            Thread myNewThread = new Thread(() => RunYoloV5InPython(PYTHON_COMMAND, PATH_TO_PY_SCRIPT, PATH_TO_WEIGHT, PATH_TO_CASE_SOURCE_ROOT, "Case0\\Source"));
            myNewThread.Start();
            
        }
        // 2 droplets fail to move
        [Test]
        public void TestCorrect1()
        {
            RunYoloV5InPython(PYTHON_COMMAND, PATH_TO_PY_SCRIPT, PATH_TO_WEIGHT, PATH_TO_CASE_SOURCE_ROOT, "Case1\\Source");

        }
    }
}