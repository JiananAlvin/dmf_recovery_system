using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExecutionEngine;

namespace ExecutionEngine
{
    internal class Program
    {
        const String IP = "localhost";
        const int PORT = 1883;
        const String TOPIC = "yolo";


        static void Main(string[] args)
        {
            // Init a hash map for storing polygonal (excluding triangular) electrodes layout
            Dictionary<int, Dictionary<int, Electrode>> layout = new Dictionary<int, Dictionary<int, Electrode>>();

            // Init a hash map for storing triangular electrodes layout
            Dictionary<int, Dictionary<int, Electrode>> layoutTri = new Dictionary<int, Dictionary<int, Electrode>>();

            // Init two maps in terms of input JSON file
            Initializer init = new Initializer();
            init.Initilalize(layout, layoutTri);

            // Subscribe YOLO output
            // Subscriber s = new Subscriber(IP, PORT);
            // s.Subscribe(TOPIC);
            // string yolo = s.GetReceivedMessage();

            // Map
            Mapper mapper = new Mapper();
            string yolo = "{ 'e_dimension': [671, 320], 'd_info': [[632.0, 239.0, 10, 12], [298.0, 353.0, 28, 30], [581.0, 310.0, 30, 32]]}";
            List<List<int>> result = mapper.Map(yolo, 860, 400, 10, layout, layoutTri); //TODO
            foreach (List<int> list in result)
            {
                Console.WriteLine(string.Join(",", list) + "\n");
            }
            
        }
    }
}
