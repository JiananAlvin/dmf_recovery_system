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
            ExecutePerformanceTest("01", 520, 520);
        }

        [Test]
        public void Test02DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("02", 520, 520);
        }


        [Test]
        public void Test03DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("03", 520, 520);
        }


        [Test]
        public void Test04DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("04", 520, 520);
        }


        [Test]
        public void Test05DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("05", 520, 520);
        }


        [Test]
        public void Test06DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("06", 520, 520);
        }


        [Test]
        public void Test07DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("07", 520, 520);
        }


        [Test]
        public void Test08DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("08", 520, 520);
        }


        [Test]
        public void Test09DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("09", 520, 520);
        }


        [Test]
        public void Test10DropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("10", 520, 520);
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
<<<<<<< HEAD
            Thread myNewThread = new Thread(() => TestUtil.PublishActStatesToMqtt(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\actual_position.txt", 30, 81));
=======
            Thread myNewThread = new Thread(() => TestUtil.PublishActStatesToMqtt(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\actual_position.txt", n, timeInterval));
>>>>>>> ef23ae8ba278c4563db4b2ff15e4c67a7fbd09d5
            myNewThread.Start();
            controller.Execute(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\recovery_result.txt", @"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\basm_result.txt", basmInstructions, expectedPositions, manager);
        }
    }
}