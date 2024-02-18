using System.Diagnostics;
using System.IO;
using Engine;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

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
    }
}