// See https://aka.ms/new-console-template for more information
using ExecutionEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Serialization;

internal class Program
{
    private static void Main(string[] args)
    {
        // Info from YOLO
        int minSize = 10;

        // Read the JSON file contents into a string
        string json = File.ReadAllText(@"../../../platform640v2.json");

        // Deserialize the JSON string into a C# object
        dynamic obj = JsonConvert.DeserializeObject(json);

        // Access object properties
        int width = obj.information.sizeX;
        int height = obj.information.sizeY;
        JArray electrodeArr = obj.electrodes;

        // Print object properties
        Console.WriteLine($"sizeX: {width}");

        // Init a hash map for storing polygonal (excluding triangular) electrodes layout
        Dictionary<int, Dictionary<int, Electrode>> layout = new Dictionary<int, Dictionary<int, Electrode>>();

        // Init a hash map for storing triangular electrodes layout
        Dictionary<int, Dictionary<int, Electrode>> layoutTri = new Dictionary<int, Dictionary<int, Electrode>>();

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
                    el.Shape = new Triangle(p1, p2, p3);
                    // Put current electrode into dictionary
                    Rec2dict(layoutTri, el, minSize, xMin, xMax, yMin, yMax);
                }
                else  // Custom polygon with sides >= 4
                {
                    // Put current electrode into dictionary
                    Rec2dict(layout, el, minSize, xMin, xMax, yMin, yMax);
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
                Rec2dict(layout, el, minSize, positionX, positionX + sizeX, positionY, positionY + sizeY);
            }
        }

        // Test
        Console.WriteLine("Please enter pixel X:");
        int xPixel = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Please enter pixel Y:");
        int yPixel = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine(Map(xPixel, yPixel, minSize, layout, layoutTri));


        /*        // Print the contents of the nested dictionary
                string result = "";
                foreach (KeyValuePair<int, Dictionary<int, Electrode>> outerDict in layout)
                {
                    foreach (KeyValuePair<int, Electrode> innerDict in outerDict.Value)
                    {
                        Console.WriteLine($"(X, Y): ({innerDict.Key},{outerDict.Key}); ID:{innerDict.Value.Id}\n");
                        result += $"(X, Y): ({innerDict.Key},{outerDict.Key}); ID:{innerDict.Value.Id}\n";
                    }
                }

                string fileName = "G:\\01_dmf_calibration_system\\ExecutionEngine\\output.txt";
                using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                using StreamWriter writerFile = new StreamWriter(fs, Encoding.UTF8);

                writerFile.Write(result);*/

        /*        // Print the contents of the nested dictionary
                string result = "";
                foreach (KeyValuePair<int, Dictionary<int, Electrode>> outerDict in layoutTri)
                {
                    foreach (KeyValuePair<int, Electrode> innerDict in outerDict.Value)
                    {
                        Console.WriteLine($"(X, Y): ({innerDict.Key},{outerDict.Key}); ID:{innerDict.Value.Id}\n");
                        result += $"(X, Y): ({innerDict.Key},{outerDict.Key}); ID:{innerDict.Value.Id}\n";
                    }
                }

                string fileName = "G:\\01_dmf_calibration_system\\ExecutionEngine\\output_tri.txt";
                using FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                using StreamWriter writerFile = new StreamWriter(fs, Encoding.UTF8);

                writerFile.Write(result);*/
    }

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

    private static int Map(int xPixel, int yPixel, int minSize, Dictionary<int, Dictionary<int, Electrode>> layout, Dictionary<int, Dictionary<int, Electrode>> layoutTri)
    {
        int keyX = (int)(xPixel / minSize) * minSize;
        int keyY = (int)(yPixel / minSize) * minSize;
        int r = -1;
        if (layoutTri.ContainsKey(keyY) && layoutTri[keyY].ContainsKey(keyX))
        {
            // See if it in the triangular electrode area
            Point p = new Point(xPixel, yPixel);
            if (layoutTri[keyY][keyX].Shape.IsPointInTriangle4(p)) {
                return layoutTri[keyY][keyX].Id;
            } 
        }

        // See if it in the polygonal electrode area
        if (layout.ContainsKey(keyY) && layout[keyY].ContainsKey(keyX))
        {
            r = layout[keyY][keyX].Id;
        }
        else
        {
            r = -1;
        }
        return r;
    }

}