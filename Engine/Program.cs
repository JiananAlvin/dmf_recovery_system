using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Test")]
namespace Engine // Note: actual namespace depends on the project name.
{
    internal class Program
    {

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

            Controller controller = new Controller();
            
            List<Tuple<List<int>, List<int>>> basmInstructions = controller.InitBasmInstructions(config["bio-assembly-src"]!);
            JArray expectedPositions = controller.InitExpectedStatus(config["expected-position"]!);

            // Read serial port
            // SelectSerialPort();
            // Clear the whole DMF chip
            SerialManager manager = new SerialManager(config["serial-port"]!, 115200);
            controller.Execute(config, basmInstructions, expectedPositions, manager);
        }
    }
}