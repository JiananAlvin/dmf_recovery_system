using Model;
using System.Drawing;
namespace UnitTest
{
    public class TestSquare
    {

        static Square s1;
        static Square s2;
        static Square s3;

        [SetUp]
        public void Setup()
        {
            s1 = new Square(0, 0, 3, 3);
            s2 = new Square(2, 0, 3, 3);
            s3 = new Square(5, 0, 1, 1);
        }

        [Test]
        public void TestCalculateArea()
        {

            Assert.That(s1.Area(), Is.EqualTo(9));
            Assert.That(s2.Area(), Is.EqualTo(9));
            Assert.That(s3.Area(), Is.EqualTo(1));
        }
        [Test]
        public void TestIntersection()
        {
            Assert.That(s1.Intersection(s2), Is.EqualTo(3));
            Assert.That(s1.Intersection(s3), Is.EqualTo(0));
            Assert.That(s2.Intersection(s3), Is.EqualTo(0));
        }

        [Test]
        public void TestIoU()
        {
            Assert.That(s1.IoU(s2), Is.EqualTo(0.2));
            Assert.That(s1.IoU(s3), Is.EqualTo(0));
            Assert.That(s2.IoU(s3), Is.EqualTo(0));
        }

    }
}