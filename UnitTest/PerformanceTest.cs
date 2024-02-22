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
            ExecutePerformanceTest("01");
        }

        [Test]
        public void TestTwoDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("02");
        }


        [Test]
        public void TestThreeDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("03");
        }


        [Test]
        public void TestFourDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("04");
        }


        [Test]
        public void TestFiveDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("05");
        }


        [Test]
        public void TestSixDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("06");
        }


        [Test]
        public void TestSevenDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("07");
        }


        [Test]
        public void TestEightDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("08");
        }


        [Test]
        public void TestNineDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("09");
        }


        [Test]
        public void TestTenDropletPerformanceBenchmark()
        {
            ExecutePerformanceTest("10");
        }



        private static void ExecutePerformanceTest(string ID)
        {
            Controller controller = new Controller();
            File.Create(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\recovery_result.txt").Close();
            File.Create(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\basm_result.txt").Close();

            List<Tuple<List<int>, List<int>>> basmInstructions = controller.InitBasmInstructions(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\exec_bio_assembly.txt");
            JArray expectedPositions = controller.InitExpectedStatus(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\expected_position.json");
            // Clear the whole DMF chip
            SerialManager manager = new SerialManager("COM4", 115200);
            // read and publish to channel 
            Thread myNewThread = new Thread(() => TestUtil.PublishActStatesToMqtt(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\actual_position.txt", 10, 500));
            myNewThread.Start();
            controller.Execute(@"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\recovery_result.txt", @"..\..\..\..\Cases\PerformanceTest-Benchmark-" + ID + @"Droplet\basm_result.txt", basmInstructions, expectedPositions, manager);
        }
    }
}