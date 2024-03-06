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
            ExecutePerformanceTest("01", 520, 520);
        }

        [Test]
        public void TestTwoDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("02", 520, 520);
        }


        [Test]
        public void TestThreeDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("03", 520, 520);
        }


        [Test]
        public void TestFourDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("04", 520, 520);
        }


        [Test]
        public void TestFiveDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("05", 520, 520);
        }


        [Test]
        public void TestSixDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("06", 520, 520);
        }


        [Test]
        public void TestSevenDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("07", 520, 520);
        }


        [Test]
        public void TestEightDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("08", 520, 520);
        }


        [Test]
        public void TestNineDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("09", 520, 520);
        }


        [Test]
        public void TestTenDropletPerformanceBenchmark()
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
            Thread myNewThread = new Thread(() => TestUtil.PublishActStatesToMqtt(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\actual_position.txt", n, timeInterval));
            myNewThread.Start();
            controller.Execute(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\recovery_result.txt", @"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\basm_result.txt", basmInstructions, expectedPositions, manager);
        }
    }
}