// See https://aka.ms/new-console-template for more information
using ExecutionEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.InteropServices;
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
            // Construct a new electrode object
            int id = (int)elInfo["ID"];
            int positionX = (int)elInfo["positionX"];
            int positionY = (int)elInfo["positionY"];
            Electrode el = new Electrode(id, positionX, positionY);

            // Other info about current eletrode
            int sizeX = (int)elInfo["sizeX"];
            int sizeY = (int)elInfo["sizeY"];
            JArray cornersJArr = (JArray)elInfo["corners"];
            // Console.WriteLine(cornersJArr + "start");
            // int[][] corners = new int[cornersJArr.Count][];

            // Rectangle electrodes
            if ((int)elInfo["shape"] == 0)
            {
                // Put current electrode into dictionary
                rec2dict(layout, el, minSize, positionX, positionX + sizeX, positionY, positionY + sizeY);
            }
            else  // Custom polygon
            { 
                if (cornersJArr.Count == 3)  // Triangle
                {
                    // Get the maximum and minimum values of the first elements of inner arrays
                    int xMax = positionX + (int)cornersJArr.Max(innerArr => innerArr[0]);
                    int xMin = positionX + (int)cornersJArr.Min(innerArr => innerArr[0]);

                    // Get the maximum and minimum values of the second elements of inner arrays
                    int yMax = positionY + (int)cornersJArr.Max(innerArr => innerArr[1]);
                    int yMin = positionY + (int)cornersJArr.Min(innerArr => innerArr[1]);

                    // Put current electrode into dictionary
                    rec2dict(layoutTri, el, minSize, xMin, xMax, yMin, yMax);
                }
            }
        }

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

        // Print the contents of the nested dictionary
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

        writerFile.Write(result);
    }

    private static void rec2dict(Dictionary<int, Dictionary<int, Electrode>> layout, Electrode el, int minSize, int xMin, int xMax, int yMin, int yMax)
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