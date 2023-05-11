using Newtonsoft.Json;

namespace ExecutionEngine
{
    public class Calibrator
    {
        public static int width;
        public static int height;
        public static int minStep;
        public static double tolerance = 1;  // This one should be user input.
        public static int sizeOfSquareEl = 20; // TODO: This should be read from JSON somehow.
        public static Dictionary<int, Dictionary<int, Electrode>> layout;
        public static Dictionary<int, Dictionary<int, Electrode>> layoutTri;

        public List<Dictionary<string, HashSet<int>>> Run(string expectedS, string actualS)
        {
            // Init two maps in terms of input JSON file
            Initializer init = new Initializer();
            init.Initilalize();
            width = init.width;
            height = init.height;
            minStep = init.minStep;
            layout = init.layout;
            layoutTri = init.layoutTri;

            /*       // Subscribe YOLO output
                   String yolo = null;
                   Subscriber s = new Subscriber(IP, PORT);
                   s.Subscribe(TOPIC);

                   while (yolo is null) 
                       {
                       yolo = s.GetReceivedMessage();
                       }

                   // Map
                   Mapper mapper = new Mapper();
                   // string yolo = "{ 'img_dimension': [671, 320], 'droplet_info': [[632.0, 239.0, 10, 12], [298.0, 353.0, 28, 30], [581.0, 310.0, 30, 32]]}";
                   // yolo is actualS
                   List<List<int>> result = mapper.Map(yolo, width, height, minSize, layout, layoutTri); //TODO


                   // overlap (expected, result)
                   // recover miss-movement
                   foreach (List<int> list in result)
                   {
                       Console.WriteLine(string.Join(",", list) + "\n");
                   }*/

            // string originalS = "[[182, 530, 100, 20, 20],       [201, 270, 120, 40, 40],       [282, 630, 140, 20, 20],       [301, 350, 180, 20, 20],       [351, 710, 200, 40, 40],       [404, 490, 240, 80, 60]]";
            // string expectedS = "[[150, 530, 80, 20, 20, 0],     [202, 290, 120, 40, 40, 1],    [283, 630, 160, 20, 20, 2],    [333, 350, 200, 20, 20, 2],    [403, 470, 240, 80, 60, 3],    [350, 690, 200, 40, 40, 3]]";  // From Wenjie's program
            // string actualS = "[[182, 530, 100, 20, 20, 0, 0], [201, 270, 120, 40, 40, 0, 0], [283, 630, 160, 20, 20, 0, 0], [301, 350, 180, 20, 20, 0, 0], [350, 690, 200, 40, 40, 0, 0], [404, 490, 240, 80, 60, 0, 0]]";
            // string actualS =   "[[182, 530, 100, 20, 20, 0, 0], [201, 270, 120, 40, 40, 0, 0], [283, 630, 160, 20, 20, 0, 0], [301, 350, 180, 20, 20, 0, 0], [350, 690, 200, 40, 40, 0, 0], [404, 490, 240, 80, 60, 0, 0]]";

            List<List<int>> statesExp = JsonConvert.DeserializeObject<List<List<int>>>(expectedS);
            List<List<int>> statesAct = JsonConvert.DeserializeObject<List<List<int>>>(actualS);

            Checker checker = new Checker();
            List<Tuple<int, int>> pairs = checker.Match(statesExp, statesAct);


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
                Console.WriteLine("E[{0}] - A[{1}], Iou is: {2}", pair.Item1, pair.Item2, ious[i]);
                i++;
            }

            // Give a list of electrodes need to be manipulated for calibration
            List<Dictionary<string, HashSet<int>>> electrodesForCalibration = checker.GetStuckRegion(Calibrator.tolerance, pairs, statesExp, statesAct);

            // Print the list of electrodes 
            Console.WriteLine("List of electrodes need to be manipulated for calibration:\n[");
            foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForCalibration)
            {
                foreach (KeyValuePair<string, HashSet<int>> kvp in elsPerDroplet)
                {
                    Console.Write("   {0}: ", kvp.Key);
                    Console.Write("[ ");
                    foreach (int num in kvp.Value)
                    {
                        Console.Write(num + " ");
                    }
                    Console.WriteLine("]");
                }
            }
            Console.WriteLine("]");

            // Save final results in "result.txt"
            string result = "";
            result += "List of electrodes need to be manipulated for calibration:\n[";
            foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForCalibration)
            {
                foreach (KeyValuePair<string, HashSet<int>> kvp in elsPerDroplet)
                {
                    result += $"   {kvp.Key}: ";
                    result += "[ ";
                    foreach (int num in kvp.Value)
                    {
                        result += num + " ";
                    }
                    result += "]";
                }
            }
            result += "   ]\n";
            result += "==========================";
            string fileName = "G:\\01_dmf_calibration_system\\ExecutionEnv\\reult.txt";

            using (StreamWriter writer = File.AppendText(fileName))
            {
                writer.WriteLine(result);
            }

            // Return the list of electrodes need to be manipulated.
            return electrodesForCalibration;
        }
    }
}
