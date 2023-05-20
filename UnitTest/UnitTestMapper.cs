using ExecutionEngine;
using System.Drawing;
namespace UnitTest
{
    public class TestMapper
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestIsPointInTriangle()
        {            
            Triangle t = new Triangle(new Point(0,0), new Point(4, 0) , new Point(0, 3));
            Assert.IsTrue(t.IsPointInTriangle(new Point(1, 1)));
        }

        [Test]
        public void TestIsNotPointInTriangle()
        {
            Triangle t = new Triangle(new Point(0, 0), new Point(4, 0), new Point(0, 3));
            Assert.IsFalse(t.IsPointInTriangle(new Point(5,5)));
        }
    }
}