using Model;
using System.Drawing;
namespace UnitTest
{
    public class TestTriangle
    {
        private Triangle t;
        [SetUp]
        public void Setup()
        {
            t = new Triangle(new Point(0, 0), new Point(4, 0), new Point(0, 3));
        }

        // point (1,1) is inside triangle 
        [Test]
        public void TestIsPointInTriangle()
        {            
            Assert.IsTrue(t.IsPointInTriangle(new Point(1, 1)));
        }


        // point (5,5) is not inside triangle 
        [Test]
        public void TestIsNotPointInTriangle()
        {
            Assert.IsFalse(t.IsPointInTriangle(new Point(5,5)));
        }


        [Test]
        public void CornerTestIsInTriangle1()
        {
            Assert.IsTrue(t.IsPointInTriangle(new Point(0, 3)));
        }

        [Test]
        public void CornerTestIsInTriangle2()
        {
            Assert.IsTrue(t.IsPointInTriangle(new Point(4, 0)));
        }
    }
}