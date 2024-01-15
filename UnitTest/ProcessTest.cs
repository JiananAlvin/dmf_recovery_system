using System.Diagnostics;
using System.IO;
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

        // Perfect match 
        [Test]
        public void TestCorrect0()
        {
            // run YoloV5
            Thread myNewThread = new Thread(() => RunYoloV5InPython(PYTHON_COMMAND, PATH_TO_PY_SCRIPT, PATH_TO_WEIGHT, PATH_TO_CASE_SOURCE_ROOT, "Case0\\Source"));
            myNewThread.Start();

            // run controller
            Controller controller = new Controller();
            File.Create(@"..\..\..\..\Cases\Case0\recovery_result.txt").Close();
            File.Create(@"..\..\..\..\Cases\Case0\basm_result.txt").Close();

            List<Tuple<List<int>, List<int>>> basmInstructions = controller.InitBasmInstructions(@"..\..\..\..\Cases\Case0\exec_bio_assembly.txt");
            JArray expectedPositions = controller.InitExpectedStatus(@"..\..\..\..\Cases\Case0\expected_position.json");
            // Clear the whole DMF chip
            SerialManager manager = new SerialManager("COM4", 115200);
            controller.Execute(@"..\..\..\..\Cases\Case0\recovery_result.txt", @"..\..\..\..\Cases\Case0\basm_result.txt", basmInstructions, expectedPositions, manager);

            string expectedRecoveryOutput = "List of electrodes need to be manipulated for recovery:\r\n[   ]\r\n\r\nList of electrodes need to be manipulated for recovery:\r\n[   ]\r\n\r\nList of electrodes need to be manipulated for recovery:\n[   ]\n\r\nList of electrodes need to be manipulated for recovery:\r\n[   ]\r\n\r\nList of electrodes need to be manipulated for recovery:\r\n[   ]\r\n\r\n".Replace("\r", "").Replace("\n", "");
            string recoveryResult = File.ReadAllText(@"..\..\..\..\Cases\Case0\recovery_result.txt").Replace("\r", "").Replace("\n", "");
            Assert.That(recoveryResult, Is.EqualTo(expectedRecoveryOutput));

            string expectedBasmOutput = "[init]:shv 1 280\r\n[init]:hvpoe 1 1\r\n[init]:clra 0\r\n[init]:clra 1\r\n[step0]:setel 1 65\r\n[step1]:setel 1 66\r\n[step2]:clrel 1 65\r\n[step3]:setel 1 67\r\n[step4]:clrel 1 66\r\n[step5]:setel 1 68\r\n[step6]:clrel 1 67\r\n[step7]:setel 1 69\r\n[step8]:clrel 1 68\r\n[step9]:setel 1 70\r\n[step10]:clrel 1 69 70\r\n[complete]:clra 0\r\n[complete]:clra 1\r\n".Replace("\r", "").Replace("\n", "");
            string basmResult = File.ReadAllText(@"..\..\..\..\Cases\Case0\basm_result.txt").Replace("\r", "").Replace("\n", "");
            Assert.That(basmResult, Is.EqualTo(expectedBasmOutput));
        }

        // 1 step error 
        [Test]
        public void TestCorrect1()
        {
            // run YoloV5
            Thread myNewThread = new Thread(() => RunYoloV5InPython(PYTHON_COMMAND, PATH_TO_PY_SCRIPT, PATH_TO_WEIGHT, PATH_TO_CASE_SOURCE_ROOT, "Case0\\Source"));
            myNewThread.Start();

            // run controller
            Controller controller = new Controller();
            File.Create(@"..\..\..\..\Cases\Case1\recovery_result.txt").Close();
            File.Create(@"..\..\..\..\Cases\Case1\basm_result.txt").Close();

            List<Tuple<List<int>, List<int>>> basmInstructions = controller.InitBasmInstructions(@"..\..\..\..\Cases\Case1\exec_bio_assembly.txt");
            JArray expectedPositions = controller.InitExpectedStatus(@"..\..\..\..\Cases\Case1\expected_position.json");
            // Clear the whole DMF chip
            SerialManager manager = new SerialManager("COM4", 115200);
            controller.Execute(@"..\..\..\..\Cases\Case1\recovery_result.txt", @"..\..\..\..\Cases\Case1\basm_result.txt", basmInstructions, expectedPositions, manager);

            string expectedRecoveryOutput = "List of electrodes need to be manipulated for recovery:\r\n[   ]\r\n\r\nList of electrodes need to be manipulated for recovery:\r\n[   ]\r\n\r\nList of electrodes need to be manipulated for recovery:\n[   ]\n\r\nList of electrodes need to be manipulated for recovery:\r\n[   ]\r\n\r\nList of electrodes need to be manipulated for recovery:\r\n[   ]\r\n\r\n".Replace("\r", "").Replace("\n", "");
            string recoveryResult = File.ReadAllText(@"..\..\..\..\Cases\Case1\recovery_result.txt").Replace("\r", "").Replace("\n", "");
            Assert.That(recoveryResult, Is.EqualTo(expectedRecoveryOutput));

            string expectedBasmOutput = "[init]:shv 1 280\r\n[init]:hvpoe 1 1\r\n[init]:clra 0\r\n[init]:clra 1\r\n[step0]:setel 1 65\r\n[step1]:setel 1 66\r\n[step2]:clrel 1 65\r\n[step3]:setel 1 67\r\n[step4]:clrel 1 66\r\n[step5]:setel 1 68\r\n[step6]:clrel 1 67\r\n[step7]:setel 1 69\r\n[step8]:clrel 1 68\r\n[step9]:setel 1 70\r\n[step10]:clrel 1 69 70\r\n[complete]:clra 0\r\n[complete]:clra 1\r\n".Replace("\r", "").Replace("\n", "");
            string basmResult = File.ReadAllText(@"..\..\..\..\Cases\Case1\basm_result.txt").Replace("\r", "").Replace("\n", "");
            Assert.That(basmResult, Is.EqualTo(expectedBasmOutput));
        }
    }
}