using ExecutionEngine;
using System.Drawing;
namespace UnitTest
{
    public class TestMapper
    {

        public double tolerance;
        public int sizeOfSquareEl;
        public String yolo = null;
        public Initializer init;
        public Mapper mapper;
        [SetUp]
        public void Setup()
        {
            tolerance = 0;
            sizeOfSquareEl = 20;
            init = new Initializer();
            init.Initilalize();
            mapper = new Mapper();
        }

        // imgSize: 860 * 400 , [110,0] is eleId 1
        [Test]
        public void TestMapperToSquare1()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[110.0,0.0,10, 12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(1));
/*            for (int i = 0; i < result.Count; i++)
            {
                Console.Write("    electrodeId': " + result[i][0] + "; ");
                Console.Write("xTopLeft': " + result[i][1] + "; ");
                Console.Write("yTopLeft': " + result[i][2] + "; ");
                Console.Write("width': " + result[i][3] + "; ");
                Console.Write("height': " + result[i][4] + "; ");
                Console.Write("xOffset': " + result[i][5] + "; ");
                Console.WriteLine("yOffset': " + result[i][6]);
            }*/
        }

        // imgSize: 860 * 400 , [110,5] is eleId 1
        [Test]
        public void TestMapperToSquare2()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[110.0,5.0,10, 12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(1));
        }

        // imgSize: 860 * 400 , [130,0] is eleId 2
        [Test]
        public void TestMapperToSquare3()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[130.0,5.0,10, 12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(2));
        }

        // imgSize: 860 * 400 , [130,5] is eleId 2
        [Test]
        public void TestMapperToSquare4()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[130.0,5.0,10, 12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(2));
        }
        // imgSize: 860 * 400 , [730,0] is eleId 32
        [Test]
        public void TestMapperToSquare5()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[730.0,0.0,10, 12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(32));
        }
        // imgSize: 860 * 400 , [730,5] is eleId 32
        [Test]
        public void TestMapperToSquare6()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[730.0,5.0,10, 12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(32));
        }

        // imgSize: 860 * 400 , [10, 255] is eleId 664
        [Test]
        public void TestMapperToPoli1()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[10.0,255.0 ,10,12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(664));
        }

        // imgSize: 860 * 400 , [40, 240] is eleId 664
        [Test]
        public void TestMapperToPoli2()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[40.0,240.0 ,10,12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(664));
        }

        // imgSize: 860 * 400 , [49, 240] is eleId 664
        [Test]
        public void TestMapperToPoli3()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[49.0,240.0 ,10,12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(664));
        }



        // imgSize: 860 * 400 , [55.0,255.0] is eleId 665
        [Test]
        public void TestMapperToTriangle1()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[55.0,255.0 ,10,12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(665));
        }

        // imgSize: 860 * 400 , [50.0,259.0] is eleId 665
        [Test]
        public void TestMapperToTriangle2()
        {
            yolo = "{ 'img_dimension': [860, 400], 'droplet_info': [[50.0,259.0 ,10,12]]}";
            List<List<int>> result = mapper.Map(yolo, init.width, init.height, init.minStep, init.layout, init.layoutTri);
            Assert.That(result[0][0], Is.EqualTo(665));
        }
    }
}
