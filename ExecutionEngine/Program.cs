using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecutionEngine;
using Newtonsoft.Json;

namespace ExecutionEngine
{
    internal class Program
    {
        const String IP = "localhost";
        const int PORT = 1883;
        const String TOPIC = "yolo";


        static void Main(string[] args)
        {
            /*        // Init two maps in terms of input JSON file
                    Initializer init = new Initializer();
                    init.Initilalize();
                    int width = init.width;
                    int height = init.height;
                    int minSize = init.minSize;
                    Dictionary<int, Dictionary<int, Electrode>> layout = init.layout;
                    Dictionary<int, Dictionary<int, Electrode>> layoutTri = init.layoutTri;

                    // Subscribe YOLO output
                    String yolo = null;
                    Subscriber s = new Subscriber(IP, PORT);
                    s.Subscribe(TOPIC);

                    while (yolo is null) 
                        {
                        yolo = s.GetReceivedMessage();
                        }

                    // Map
                    Mapper mapper = new Mapper();
                    // string yolo = "{ 'e_dimension': [671, 320], 'd_info': [[632.0, 239.0, 10, 12], [298.0, 353.0, 28, 30], [581.0, 310.0, 30, 32]]}";
                    // yolo is actualS
                    List<List<int>> result = mapper.Map(yolo, width, height, minSize, layout, layoutTri); //TODO


                    // overlap (expected, result)
                    // recover miss-movement
                    foreach (List<int> list in result)
                    {
                        Console.WriteLine(string.Join(",", list) + "\n");
                    }*/

            string expectedS = "[[100, 1, 2, 3, 7, 0, 0], [101, 4, 5, 6, 7, 0, 0], [102, 7, 8, 40, 9, 0, 0]]";  // From Wenjie's program
            string actualS = "[[103, 2, 3, 4, 6, 0], [104, 8, 9, 1, 9, 0], [105, 5, 6, 23, 45, 0, 0]]";

            List<List<int>> statesExp = JsonConvert.DeserializeObject<List<List<int>>>(expectedS);
            List<List<int>> statesAct = JsonConvert.DeserializeObject<List<List<int>>>(actualS);

            Checker checker = new Checker();
            List<Tuple<int, int>> pairs = checker.Compare(statesExp, statesAct);


            // Computes overlap
            List<double> ious = new List<double>();
            foreach (Tuple<int, int> pair in pairs)
            {
                Square squareExp = new Square(statesExp[pair.Item1][1], statesExp[pair.Item1][2], statesExp[pair.Item1][3], statesExp[pair.Item1][4]);
                Square squareAct = new Square(statesAct[pair.Item2][1], statesAct[pair.Item2][2], statesAct[pair.Item2][3], statesAct[pair.Item2][4]);
                double iou = squareExp.IoU(squareAct);
                ious.Add(iou);
            }

            // Print pairs and IoU
            int i = 0;
            foreach (Tuple<int, int> pair in pairs)
            {
                Console.WriteLine("A[{0}] - B[{1}], Iou is: {2}", pair.Item1, pair.Item2, ious[i]);
                i++;
            }
        }
    }
}
