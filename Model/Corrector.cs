using Newtonsoft.Json;

namespace Model
{
    public class Corrector
    {
        public double tolerance = 0.6;  // This one should be user input.
        public Initializer init;

        public Corrector()
        {
            init = new Initializer();
            init.Initilalize();
        }

        public List<Dictionary<string, HashSet<int>>> Run(string expectedS, string actualS, string output)
        {
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
            
            // Give a list of electrodes need to be manipulated for recovery
            List<Dictionary<string, HashSet<int>>> electrodesForRecovery = checker.GetStuckRegion(tolerance, pairs, statesExp, statesAct, init.nonTriangleHashMap, init.triangleHashMap, init.minStep, init.sizeOfSquareEl);

            // Print the list of electrodes 
            Console.WriteLine("List of electrodes need to be manipulated for recovery:\n[");
            foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
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
            result += "List of electrodes need to be manipulated for recovery:\n[";
            foreach (Dictionary<string, HashSet<int>> elsPerDroplet in electrodesForRecovery)
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

            File.AppendAllText(output, result);

            // Return the list of electrodes need to be manipulated.
            return electrodesForRecovery;
        }
    }
}
