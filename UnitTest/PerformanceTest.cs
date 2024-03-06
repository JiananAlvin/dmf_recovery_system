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
        public void Test01DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("01", 10, 478);
        }

        [Test]
        public void Test02DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("02", 10, 471);
        }


        [Test]
        public void Test03DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("03", 10, 471);
        }


        [Test]
        public void Test04DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("04", 10, 464);
        }


        [Test]
        public void Test05DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("05", 10, 464);
        }


        [Test]
        public void Test06DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("06", 10, 477);
        }


        [Test]
        public void Test07DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("07", 10, 458);
        }


        [Test]
        public void Test08DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("08", 10, 471);
        }


        [Test]
        public void Test09DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("09", 10, 469);
        }


        [Test]
        public void Test10DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("10", 10, 474);
        }



        private static void ExecutePerformanceTest(string ID, int n, int timeInterval)
        {
            Controller controller = new Controller();
            File.Create(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\recovery_result.txt").Close();
            File.Create(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\basm_result.txt").Close();

            List<Tuple<List<int>, List<int>>> basmInstructions = controller.InitBasmInstructions(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\exec_bio_assembly.txt");
            JArray expectedPositions = controller.InitExpectedStatus(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\expected_position.json");
            // Clear the whole DMF chip
            SerialManager manager = new SerialManager("COM4", 115200);
            // read and publish to channel 
            Thread myNewThread = new Thread(() => TestUtil.PublishActStatesToMqtt(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\actual_position.txt", n, timeInterval));
            myNewThread.Start();
            controller.Execute(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\recovery_result.txt", @"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\basm_result.txt", basmInstructions, expectedPositions, manager);
        }
    }
}