﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionEngine
{
    public class Part1Tester
    {
        const String IP = "localhost";
        const int PORT = 1883;
        internal string TOPIC = "yolo/det";
        public static int width;
        public static int height;
        public static int minStep;
        public static double tolerance = 1;  // This one should be user input.
        public static int sizeOfSquareEl = 20; // TODO: This should be read from JSON somehow.
        public static Dictionary<int, Dictionary<int, Electrode>> layout;
        public static Dictionary<int, Dictionary<int, Electrode>> layoutTri;
        public static String yolo = null;

        public void Run() 
        {
            // Init two maps in terms of input JSON file
            Initializer init = new Initializer();
            init.Initilalize();
            width = init.width;
            height = init.height;
            minStep = init.minStep;
            layout = init.layout;
            layoutTri = init.layoutTri;

            // Subscribe YOLO output
            // String yolo = null;
            Subscriber s = new Subscriber(IP, PORT);
            s.Subscribe(TOPIC);

            /*
            while (yolo is null)
            {
                yolo = s.GetReceivedMessage();
            }*/

            // Map
            Mapper mapper = new Mapper();
            // string yolo = "{ 'e_dimension': [671, 320], 'd_info': [[632.0, 239.0, 10, 12], [298.0, 353.0, 28, 30], [581.0, 310.0, 30, 32]]}";

            while (true) {
                // yolo is actualS
                List<List<int>> result = mapper.Map(yolo, width, height, minStep, layout, layoutTri); //TODO


                // overlap (expected, result)
                // recover miss-movement
                foreach (List<int> list in result)
                {
                    Console.WriteLine(string.Join(",", list) + "\n");
                }
            }
        }
    }
}
