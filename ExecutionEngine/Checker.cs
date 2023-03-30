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
using static System.Runtime.InteropServices.JavaScript.JSType;
using Point = System.Drawing.Point;

namespace ExecutionEngine
{
    internal class IntegerWrapper
    {
        internal int content { get; set; }

        public IntegerWrapper(int i) { this.content = i; }
    }

    internal class Checker
    {
        //  Uses kdTree to find the nearest neighbor pair between two lists of points
        // It takes O(n log n).
        public List<Tuple<int, int>> Match(List<List<int>> statesExp, List<List<int>> statesAct)
        {  
            KdTree<IntegerWrapper> treeExp = BuildKdTree(statesExp);
            KdTree<IntegerWrapper> treeAct = BuildKdTree(statesAct);

            List<Tuple<int, int>> pairs = new List<Tuple<int, int>>();
            for (int indexExp = 0; indexExp < treeExp.Count; indexExp++)
            {
                Coordinate coordExp = new Coordinate(statesExp[indexExp][1], statesExp[indexExp][2]);
                // NearestNeighbor() takes O(log n), where n is the number of nodes in the tree
                int indexAct = treeAct.NearestNeighbor(coordExp).Data.content;  
                Tuple<int, int> pair = new Tuple<int, int>(indexExp, indexAct);
                pairs.Add(pair);
            }
            return pairs;  // (expected, actual)
        }


        public KdTree<IntegerWrapper> BuildKdTree(List<List<int>> S)
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

        // expectedS: [electrod id of left top, left top x coord, left top y coord, width, height, direction] 
        // actualS:   [electrod id of left top, left top x coord, left top y coord, width, height, xoffset, yoffset]
        public List<HashSet<int>> GetStuckRegion(double tolerance, List<Tuple<int, int>> pairs, List<List<int>> statesExp, List<List<int>> statesAct)
        {
            List<HashSet<int>> stuckRegionPerFrame = new List<HashSet<int>>();
            foreach (Tuple<int, int> pair in pairs)
            {
                Square squareExp = new Square(statesExp[pair.Item1][1], statesExp[pair.Item1][2], statesExp[pair.Item1][3], statesExp[pair.Item1][4]);
                Square squareAct = new Square(statesAct[pair.Item2][1], statesAct[pair.Item2][2], statesAct[pair.Item2][3], statesAct[pair.Item2][4]);

                if (IsStuck(tolerance, squareAct.IoU(squareAct), statesExp[pair.Item1]))
                {
                    List<int> stuckDropletInfo = statesExp[pair.Item1];
                    int xtl = stuckDropletInfo[1];
                    int ytl = stuckDropletInfo[2];
                    int width = stuckDropletInfo[3];
                    int height = stuckDropletInfo[4];
                    // 0 up, 1 right, 2 down, 3 left
                    int direction = stuckDropletInfo[5];
                    int y;
                    int x;
                    HashSet<int> stuckRegionPerDroplet = new HashSet<int>();
                    Mapper mapper = new Mapper();
                    switch (direction)
                    {
                        case 0:  // Up
                            y = ytl + height;
                            for (x = xtl; x < xtl + width; x += Program.minStep)
                            {
                                Electrode el = mapper.GetElectrode(x, y, Program.minStep, Program.layout, Program.layoutTri);
                                stuckRegionPerDroplet.Add(el.Id);
                            }
                            break;
                        case 1:  // Right
                            x = xtl - Program.minStep;
                            for (y = ytl; y < ytl + height; y += Program.minStep)
                            {
                                Electrode el = mapper.GetElectrode(x, y, Program.minStep, Program.layout, Program.layoutTri);
                                stuckRegionPerDroplet.Add(el.Id);
                            }
                            break;
                        case 2:  // Down
                            y = ytl - Program.minStep;
                            for (x = xtl; x < xtl + width; x += Program.minStep)
                            {
                                Electrode el = mapper.GetElectrode(x, y, Program.minStep, Program.layout, Program.layoutTri);
                                stuckRegionPerDroplet.Add(el.Id);
                            }
                            break;
                        case 3:  // Left
                            x = xtl + width;
                            for (y = ytl; y < ytl + height; y += Program.minStep)
                            {
                                Electrode el = mapper.GetElectrode(x, y, Program.minStep, Program.layout, Program.layoutTri);
                                stuckRegionPerDroplet.Add(el.Id);
                            }
                            break;
                        default:
                            break;
                    }
                    // If a droplet is stuck, add a list of expected droplets that the droplet is on to 'stuckRegionPerFrame'
                    // [[a list of expected droplets that the droplet1 is on], [droplet2], [droplet3]]
                    stuckRegionPerFrame.Add(stuckRegionPerDroplet);
                }
            }
            return stuckRegionPerFrame;
        }

        public bool IsStuck(double tolerance, double actualIou, List<int> stateExp)
        {
            double iouOfPerfectMove;
            // If the droplet wanna goes up or down, then we are interested in height
            if (stateExp[5] == 0 || stateExp[5] == 2)
            {
                iouOfPerfectMove = (stateExp[4] - Program.height) / (stateExp[4] + Program.height);
            }
            else // If the droplet wanna goes left or right, then we are interested in width
            {
                iouOfPerfectMove = (stateExp[3] - Program.width) / (stateExp[3] + Program.width);
            }
            return actualIou > tolerance * iouOfPerfectMove;
        }
    }
}
