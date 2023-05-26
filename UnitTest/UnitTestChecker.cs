using ExecutionEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Drawing;
namespace UnitTest
{
    public class TestChecker
    {

        [SetUp]
        public void Setup()
        {
        }

        // all droplets move successfully
        [Test]
        public void TestCorrect1()
        {
            Corrector corrector = new Corrector();
            List<Dictionary<string, HashSet<int>>> electrodesForRecovery;

            string expectedStates = "[ " +
                "      [ 342, 530, 200, 40, 40, 2 ]," +
                "      [ 398, 370, 240, 40, 40, 3 ]," +
                "      [ 201, 270, 120, 20, 20, 1 ]" +
                "    ] ";
            string actualStates = "[\r\n      [ 342, 530, 200, 40, 40, 0, 5 ],\r\n      [ 398, 371, 240, 40, 40, 1, 0 ],\r\n      [ 201, 270, 120, 20, 20, 3, 2 ]\r\n    ]";
            electrodesForRecovery = corrector.Run(expectedStates, actualStates, "..\\..\\..\\..\\ExecutionEnv\\recovery_output.txt");

            // If correction result is an empty list (i.e. Actual states match expected states), then give "okay" to executor.
            Assert.That(electrodesForRecovery.Count, Is.EqualTo(0));
        }

        // 2 droplets fail to move
        [Test]
        public void TestCorrect2()
        {
            Corrector corrector = new Corrector();
            List<Dictionary<string, HashSet<int>>> electrodesForRecovery;

            string expectedStates = "[ " +
                "      [ 342, 530, 200, 40, 40, 2 ]," +
                "      [ 398, 370, 240, 40, 40, 3 ]," +
                "      [ 201, 270, 120, 20, 20, 1 ]" +
                "    ] ";
            string actualStates = "[\r\n      [ 310, 530, 185, 39, 41, 0, 5 ],\r\n      [ 398, 371, 240, 40, 40, 1, 0 ],\r\n      [ 200, 253, 122, 22, 17, 3, 2 ]\r\n    ]";
                electrodesForRecovery = corrector.Run(expectedStates, actualStates, "..\\..\\..\\..\\ExecutionEnv\\recovery_output.txt");

            // If correction result is an empty list (i.e. Actual states match expected states), then give "okay" to executor.
            Assert.That(electrodesForRecovery.Count, Is.EqualTo(2));
        }
    }
}