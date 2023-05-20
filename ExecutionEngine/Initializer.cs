using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Drawing;

namespace ExecutionEngine
{
    public class Initializer
    {
        public int width { get; set; }
        public int height { get; set; }
        public int minStep { get; set; }
        public Dictionary<int, Dictionary<int, Electrode>> layout { get; set; } = new Dictionary<int, Dictionary<int, Electrode>>();
        public Dictionary<int, Dictionary<int, Electrode>> layoutTri { get; set; } = new Dictionary<int, Dictionary<int, Electrode>>();
        public int sizeOfSquareEl = 20; // TODO: This should be read from JSON somehow.

        public void Initilalize()
        {
            // Info from YOLO
            int minSize = 10;

            // Read the JSON file contents into a string
            string json = File.ReadAllText(@"../../../platform640v2.json");

            // Deserialize the JSON string into a C# object
            dynamic obj = JsonConvert.DeserializeObject(json);

            // Access object properties
            this.width = obj.information.sizeX;
            this.height = obj.information.sizeY;
            this.minStep = obj.information.minStep;
            JArray electrodeArr = obj.electrodes;

            // Print object properties
            // Console.WriteLine($"sizeX: {width}");

            // Traverse all electrodes
            foreach (JObject elInfo in electrodeArr)
            {
                if ((int)elInfo["shape"] == 1) // Custom polygon
                {
                    // Construct a new electrode object
                    int id = (int)elInfo["ID"];
                    int positionX = (int)elInfo["positionX"];
                    int positionY = (int)elInfo["positionY"];
                    Electrode el = new Electrode(id, positionX, positionY);

                    // Other info about current eletrode
                    int sizeX = (int)elInfo["sizeX"];
                    int sizeY = (int)elInfo["sizeY"];
                    JArray cornersJArr = (JArray)elInfo["corners"];

                    // Get the maximum and minimum values of the first elements of inner arrays
                    int xMax = positionX + (int)cornersJArr.Max(innerArr => innerArr[0]);
                    int xMin = positionX + (int)cornersJArr.Min(innerArr => innerArr[0]);

                    // Get the maximum and minimum values of the second elements of inner arrays
                    int yMax = positionY + (int)cornersJArr.Max(innerArr => innerArr[1]);
                    int yMin = positionY + (int)cornersJArr.Min(innerArr => innerArr[1]);

                    if (cornersJArr.Count == 3)  // Triangle
                    {
                        Point p1 = new Point((int)cornersJArr[0][0] + positionX, (int)cornersJArr[0][1] + positionY);
                        Point p2 = new Point((int)cornersJArr[1][0] + positionX, (int)cornersJArr[1][1] + positionY);
                        Point p3 = new Point((int)cornersJArr[2][0] + positionX, (int)cornersJArr[2][1] + positionY);
                        el.Triangle = new Triangle(p1, p2, p3);
                        // Put current electrode into dictionary
                        Rec2dict(this.layoutTri, el, minSize, xMin, xMax, yMin, yMax);
                    }
                    else  // Custom polygon with sides >= 4
                    {
                        // Put current electrode into dictionary
                        Rec2dict(this.layout, el, minSize, xMin, xMax, yMin, yMax);
                    }
                }
            }

            // Traverse all electrodes, and overwrite "the custom polygons with sides >= 4"
            foreach (JObject elInfo in electrodeArr)
            {
                // Rectangular electrodes
                if ((int)elInfo["shape"] == 0)
                {
                    // Construct a new electrode object
                    int id = (int)elInfo["ID"];
                    int positionX = (int)elInfo["positionX"];
                    int positionY = (int)elInfo["positionY"];
                    Electrode el = new Electrode(id, positionX, positionY);

                    // Other info about current eletrode
                    int sizeX = (int)elInfo["sizeX"];
                    int sizeY = (int)elInfo["sizeY"];
                    JArray cornersJArr = (JArray)elInfo["corners"];

                    // Put current electrode into dictionary
                    Rec2dict(this.layout, el, minSize, positionX, positionX + sizeX, positionY, positionY + sizeY);
                }
            }
        }

        // TODO: TEST
        private static void Rec2dict(Dictionary<int, Dictionary<int, Electrode>> layout, Electrode el, int minSize, int xMin, int xMax, int yMin, int yMax)
        {
            int keyY = yMin;
            while (keyY < yMax)
            {
                int keyX = xMin;
                while (keyX < xMax)
                {
                    if (!layout.ContainsKey(keyY))
                    {
                        layout[keyY] = new Dictionary<int, Electrode>();
                    }
                    layout[keyY][keyX] = el;
                    keyX += minSize;
                }
                keyY += minSize;
            }
        }
    }
}
