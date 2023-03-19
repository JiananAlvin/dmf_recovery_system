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
    internal class Program
    {
        // Uses kdTree to find the nearest neighbor pair between two lists of points
        public static List<Tuple<int, int>> Compare(string expectedStates, string actualStates)
        {   
            List<List<int>> StatesExp = JsonConvert.DeserializeObject<List<List<int>>>(expectedStates);
            List<List<int>> StatesAct = JsonConvert.DeserializeObject<List<List<int>>>(actualStates);

            KdTree<Coordinate> treeExp = BuildKdTree(StatesExp);
            KdTree<Coordinate> treeAct = BuildKdTree(StatesAct);

            List<Tuple<int, int>> pairs = new List<Tuple<int, int>>();
            for (int indexExp = 0; indexExp < treeExp.Count; indexExp++)
            {
                var coordExp = new Coordinate(StatesExp[indexExp][0], StatesExp[indexExp][1]);
                var coordAct = treeAct.NearestNeighbor(coordExp);
                // Console.WriteLine(coordAct.Coordinate);
                // FindIndex takes O(n) time!!!! TODO: need modify!!!
                int indexAct = StatesAct.FindIndex(b => b[0] == coordAct.X && b[1] == coordAct.Y);  // Finds the index of corresponding actual point
                var pair = new Tuple<int, int>(indexExp, indexAct);
                pairs.Add(pair);
            }
            return pairs;
        }


        public static KdTree<Coordinate> BuildKdTree(List<List<int>> S)
        {
            // Create points
            KdTree<Coordinate> treeS = new KdTree<Coordinate>(2);
            for (int i = 0; i < S.Count; i++)
            {
                var point = new Coordinate(S[i][0], S[i][1]);
                treeS.Insert(point);
            }
            return treeS;
        }

        static void Main(string[] args)
        {

            string expectedS = "[[1, 2, 3, 7], [4, 5, 6, 7], [7, 8, 40, 9]]";
            string actualS = "[[2, 3, 4, 6], [8, 9, 1, 9], [5, 6, 23, 45]]";

            List<Tuple<int, int>> pairs = Compare(expectedS, actualS);

            // Print pairs
            foreach (Tuple<int, int> pair in pairs)
            {
                Console.WriteLine("A[{0}] - B[{1}]", pair.Item1, pair.Item2);
            }
        }
    }
}
