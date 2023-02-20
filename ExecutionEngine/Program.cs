// See https://aka.ms/new-console-template for more information
using ExecutionEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;

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

        // Init a hash map for storing electrodes layout
        Dictionary<int, Dictionary<int, Electrode>> layout = new Dictionary<int, Dictionary<int, Electrode>>();

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
            JArray cornersArr = (JArray)elInfo["corners"];
            int[][] corners = new int[cornersArr.Count][];

            // Square electrodes
            if ((int)elInfo["shape"] == 0)
            {
                int keyY = positionY;
                while (keyY < positionY + sizeY)
                {
                    int keyX = positionX;
                    while (keyX < positionX + sizeX)
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

        // Print the contents of the nested dictionary
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

        writerFile.Write(result);
    }
}