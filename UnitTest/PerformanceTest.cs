using Engine;
using Newtonsoft.Json.Linq;

namespace UnitTest
{
    public class PerformanceTest
    {
        [SetUp]
        public void Setup()
        {
        }

        // Perfect match 
        [Test]
        public void TestOneDropletPerformanceBenchmark()
        {
            // run controller
            Controller controller = new Controller();
            File.Create(@"..\..\..\..\Cases\PerformanceTest-Benchmark-OneDroplet\recovery_result.txt").Close();
            File.Create(@"..\..\..\..\Cases\PerformanceTest-Benchmark-OneDroplet\basm_result.txt").Close();

            List<Tuple<List<int>, List<int>>> basmInstructions = controller.InitBasmInstructions(@"..\..\..\..\Cases\PerformanceTest-Benchmark-OneDroplet\exec_bio_assembly.txt");
            JArray expectedPositions = controller.InitExpectedStatus(@"..\..\..\..\Cases\PerformanceTest-Benchmark-OneDroplet\expected_position.json");
            // Clear the whole DMF chip
            SerialManager manager = new SerialManager("COM4", 115200);
            // read and publish to channel 
            Thread myNewThread = new Thread(() => TestUtil.PublishActStatesToMqtt(@"..\..\..\..\Cases\PerformanceTest-Benchmark-OneDroplet\actual_position.txt", 10,500));
            myNewThread.Start();
            controller.Execute(@"..\..\..\..\Cases\PerformanceTest-Benchmark-OneDroplet\recovery_result.txt", @"..\..\..\..\Cases\PerformanceTest-Benchmark-OneDroplet\basm_result.txt", basmInstructions, expectedPositions, manager);
        }

    }
}