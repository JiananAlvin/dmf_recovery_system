using Microsoft.Extensions.Configuration;
using System.Threading;

namespace Engine // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        /*
         args:
        --platform-setting : The json file of the description of the platform
        --expected-position : The file describing the expected positions
        --bio-assembly-src : The bio assembly source file (protocal)
        */
        static void Main(string[] args)
        {

            // read args
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);

            var config = builder.Build();

            Console.WriteLine($"platform-setting: '{config["platform-setting"]}'");
            Console.WriteLine($"expected-position: '{config["expected-position"]}'");
            Console.WriteLine($"bio-assembly-src: '{config["bio-assembly-src"]}'");


            // read all lines from file
            string[] lines = File.ReadAllLines(config["bio-assembly-src"]!);

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

            // output 
            foreach (var group in groupedBasmInstruction)
            {
                Thread.Sleep(1000);
                Console.WriteLine("Group:");
                foreach (var line in group)
                {
                    Console.WriteLine(line);
                }
                Console.WriteLine();
            }

        }
    }
}