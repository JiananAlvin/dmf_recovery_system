using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecutionEngine;

namespace ExecutionEngine
{
    internal class Program1
    {
        const String IP = "localhost";
        const int PORT = 1883;
        const String TOPIC = "yolo";


        static void Main1(string[] args)
        {
        // Init two maps in terms of input JSON file
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
        List<List<int>> result = mapper.Map(yolo, width, height, minSize, layout, layoutTri); //TODO
        foreach (List<int> list in result)
        {
            Console.WriteLine(string.Join(",", list) + "\n");
        }
        }
    }
}
