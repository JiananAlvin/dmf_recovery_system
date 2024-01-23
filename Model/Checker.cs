using NetTopologySuite.Geometries;
using NetTopologySuite.Index.KdTree;

namespace Model
{
    internal class IntegerWrapper
    {
        internal int content { get; set; }

        public IntegerWrapper(int i) { this.content = i; }
    }

    internal class Checker
    {
        // Uses kdTree to find the nearest neighbor pair between two lists of points
        // It takes O(n log n).
        public List<Tuple<int, int>> Match(List<List<int>> statesExp, List<List<int>> statesAct)
        {  
            KdTree<IntegerWrapper> treeAct = BuildKdTree(statesAct);

            List<Tuple<int, int>> pairs = new List<Tuple<int, int>>();
            for (int indexExp = 0; indexExp < statesExp.Count; indexExp++)
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
                treeS.Insert(point, new IntegerWrapper(i));
            }
            return treeS;
        }

        // expectedS: [electrod id of left top, left top x coord, left top y coord, width, height, direction] 
        // actualS:   [electrod id of left top, left top x coord, left top y coord, width, height, xoffset, yoffset]
        public List<Dictionary<string, HashSet<int>>> GetStuckRegion(double tolerance, List<Tuple<int, int>> pairs, List<List<int>> statesExp, List<List<int>> statesAct, Dictionary<int, Dictionary<int, Electrode>> nonTriangleHashMap, Dictionary<int, Dictionary<int, Electrode>> triangleHashMap, int minStep, int sizeOfSquareEl)
        {
            List<Dictionary<string, HashSet<int>>> elsPerFrame = new List<Dictionary<string, HashSet<int>>>();
            foreach (Tuple<int, int> pair in pairs)
            {
                Square squareExp = new Square(statesExp[pair.Item1][1], statesExp[pair.Item1][2], statesExp[pair.Item1][3], statesExp[pair.Item1][4]);
                Square squareAct = new Square(statesAct[pair.Item2][1], statesAct[pair.Item2][2], statesAct[pair.Item2][3], statesAct[pair.Item2][4]);
             
                if (IsStuck(tolerance, squareExp.IntersectionOverExpectation(squareAct), statesExp[pair.Item1], sizeOfSquareEl))
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
                    HashSet<int> headEls = new HashSet<int>();
                    HashSet<int> tailEls = new HashSet<int>();
                    Dictionary<string, HashSet<int>> elsPerDroplet = new Dictionary<string, HashSet<int>>();
                    Mapper mapper = new Mapper();
                    switch (direction)
                    {
                        case 0:  // Up
                            for (x = xtl; x < xtl + width; x += minStep)
                            {
                                Electrode tailEl = mapper.GetElectrode(x, ytl + height, minStep, nonTriangleHashMap, triangleHashMap);
                                tailEls.Add(tailEl.Id);
                                Electrode headEl = mapper.GetElectrode(x, ytl, minStep, nonTriangleHashMap, triangleHashMap);
                                headEls.Add(headEl.Id);
                            }
                            break;
                        case 1:  // Right
                            for (y = ytl; y < ytl + height; y += minStep)
                            {
                                Electrode tailEl = mapper.GetElectrode(xtl - minStep, y, minStep, nonTriangleHashMap, triangleHashMap);
                                tailEls.Add(tailEl.Id);
                                Electrode headEl = mapper.GetElectrode(xtl + width - minStep, y, minStep, nonTriangleHashMap, triangleHashMap);
                                headEls.Add(headEl.Id);
                            }
                            break;
                        case 2:  // Down
                            for (x = xtl; x < xtl + width; x += minStep)
                            {
                                Electrode tailEl = mapper.GetElectrode(x, ytl - minStep, minStep, nonTriangleHashMap, triangleHashMap);
                                tailEls.Add(tailEl.Id);
                                Electrode headEl = mapper.GetElectrode(x, ytl + height - minStep, minStep, nonTriangleHashMap, triangleHashMap);
                                headEls.Add(headEl.Id);
                            }
                            break;
                        case 3:  // Left
                            for (y = ytl; y < ytl + height; y += minStep)
                            {
                                Electrode tailEl = mapper.GetElectrode(xtl + width, y, minStep, nonTriangleHashMap, triangleHashMap);
                                tailEls.Add(tailEl.Id);
                                Electrode headEl = mapper.GetElectrode(xtl, y, minStep, nonTriangleHashMap, triangleHashMap);
                                headEls.Add(headEl.Id);
                            }
                            break;
                        default:
                            break;
                    }
                    // If a droplet is stuck, add a list of expected droplets that the droplet is on to 'stuckRegionPerFrame'
                    // [[a list of expected droplets that the droplet1 is on], [droplet2], [droplet3]]
                    elsPerDroplet.Add("head", headEls);
                    elsPerDroplet.Add("tail", tailEls);
                    elsPerFrame.Add(elsPerDroplet);
                }
            }
            return elsPerFrame;
        }

        public bool IsStuck(double tolerance, double actExpIou, List<int> stateExp, int sizeOfSquareEl)
        {
/*            double iouOfPerfectMove;
            if (stateExp[5] == 0 || stateExp[5] == 2)
            {
                iouOfPerfectMove = (double)(stateExp[4] - sizeOfSquareEl) / (double)(stateExp[4] + sizeOfSquareEl);
            }
            else // If the droplet wanna goes left or right, then we are interested in width
            {   // TODO: Corrector.sizeOfSquareEl should be size of exp left corner size !!!
                iouOfPerfectMove = (double)(stateExp[3] - sizeOfSquareEl) / (double)(stateExp[3] + sizeOfSquareEl);
            }
            // The rhs is the minimum acceptable actual IoU
            return actExpIou < iouOfPerfectMove + (1 - tolerance) * (1 - iouOfPerfectMove);*/
return  actExpIou < tolerance;
        }
    }
}
