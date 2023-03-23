using NetTopologySuite.Geometries;
using NetTopologySuite.Index.KdTree;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Point = System.Drawing.Point;

namespace ExecutionEngine
{
    internal class IntegerWrapper
    {
        internal int content { get; set; }

        public IntegerWrapper(int i) { this.content = i; }
    }

    internal class Program
    {
        // Uses kdTree to find the nearest neighbor pair between two lists of points
        // It takes O(n log n).
        public static List<Tuple<int, int>> Compare(List<List<int>> StatesExp, List<List<int>> StatesAct)
        {  
            KdTree<IntegerWrapper> treeExp = BuildKdTree(StatesExp);
            KdTree<IntegerWrapper> treeAct = BuildKdTree(StatesAct);

            List<Tuple<int, int>> pairs = new List<Tuple<int, int>>();
            for (int indexExp = 0; indexExp < treeExp.Count; indexExp++)
            {
                Coordinate coordExp = new Coordinate(StatesExp[indexExp][1], StatesExp[indexExp][2]);
                // NearestNeighbor() takes O(log n), where n is the number of nodes in the tree
                int indexAct = treeAct.NearestNeighbor(coordExp).Data.content;  
                Tuple<int, int> pair = new Tuple<int, int>(indexExp, indexAct);
                pairs.Add(pair);
            }
            return pairs;  // (expected, actual)
        }


        public static KdTree<IntegerWrapper> BuildKdTree(List<List<int>> S)
        {
            // Create points
            KdTree<IntegerWrapper> treeS = new KdTree<IntegerWrapper>(2);
            for (int i = 0; i < S.Count; i++)
            {
                Coordinate point = new Coordinate(S[i][1], S[i][2]);
                // treeS.Insert(point);
                treeS.Insert(point, new IntegerWrapper(i));
            }
            return treeS;
        }

        static void Main(string[] args)
        {

            string expectedS = "[[100, 1, 2, 3, 7, 0, 0], [101, 4, 5, 6, 7, 0, 0], [102, 7, 8, 40, 9, 0, 0]]";  // From Wenjie's program
            string actualS = "[[103, 2, 3, 4, 6, 0], [104, 8, 9, 1, 9, 0], [105, 5, 6, 23, 45, 0, 0]]";

            List<List<int>> statesExp = JsonConvert.DeserializeObject<List<List<int>>>(expectedS);
            List<List<int>> statesAct = JsonConvert.DeserializeObject<List<List<int>>>(actualS);

            List<Tuple<int, int>> pairs = Compare(statesExp, statesAct);

            // Print pairs
/*            foreach (Tuple<int, int> pair in pairs)
            {
                Console.WriteLine("A[{0}] - B[{1}]", pair.Item1, pair.Item2);
            }*/

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
