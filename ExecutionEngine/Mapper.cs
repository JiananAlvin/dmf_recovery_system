// See https://aka.ms/new-console-template for more information
using ExecutionEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Serialization;

public class Mapper
{
    // Maps image-based droplets' imformation to simulator-based droplets' information
    // the out put format is [[eltrodeID, xTopLeft, yTopLeft, width, height, xoffset, yoffset], [], [], ...]
    public List<List<int>> Map(string yolo, int chipWidth, int chipHeight, int minSize, Dictionary<int, Dictionary<int, Electrode>> layout, Dictionary<int, Dictionary<int, Electrode>> layoutTri)
    {
        // yolo = "{ 'img_dimension': [671, 320], 'droplet_info': [[632.0, 239.0, 10, 12], [298.0, 353.0, 28, 30], [581.0, 310.0, 30, 32]]}";
        dynamic json = JsonConvert.DeserializeObject(yolo);
        int picWidth = (int)json.img_dimension[0];
        int picHeight = (int)json.img_dimension[1];
        float widthRatio = (float)chipWidth / picWidth;
        float heightRatio = (float)chipHeight / picHeight;

        List<List<int>> output = new List<List<int>>();
        foreach (JArray dropletInfo in json.droplet_info)
        {
            List<int> info = new List<int>();
            Electrode el = GetElectrode((int)dropletInfo[0], (int)dropletInfo[1], minSize, layout, layoutTri);
            Console.WriteLine("el is" + el);
            int elNo = el.Id;
            int xTopLeft = (int)Math.Round((int)dropletInfo[0] * widthRatio);
            int yTopLeft = (int)Math.Round((int)dropletInfo[1] * heightRatio);
            int dropletWidth = (int)Math.Round((int)dropletInfo[2] * widthRatio);
            int dropletHeight = (int)Math.Round((int)dropletInfo[3] * heightRatio);
            int xOffset = (int)Math.Round((int)dropletInfo[0] * widthRatio) - el.PositionX;
            int yOffset = (int)Math.Round((int)dropletInfo[1] * heightRatio) - el.PositionY;
            info.Add(elNo);
            info.Add(xTopLeft);
            info.Add(yTopLeft);
            info.Add(dropletWidth);
            info.Add(dropletHeight);
            info.Add(xOffset);
            info.Add(yOffset);
            output.Add(info);
        }
        return output;
    }

    public Electrode GetElectrode(int xPixel, int yPixel, int minSize, Dictionary<int, Dictionary<int, Electrode>> layout, Dictionary<int, Dictionary<int, Electrode>> layoutTri)
    {
        int keyX = (int)(xPixel / minSize) * minSize;
        int keyY = (int)(yPixel / minSize) * minSize;
        Console.WriteLine("keyX: " + keyX);
        Console.WriteLine("keyY: " + keyY);
        if (layoutTri.ContainsKey(keyY) && layoutTri[keyY].ContainsKey(keyX))
        {
            // See if it in the triangular electrode area
            Point p = new Point(xPixel, yPixel);
            if (layoutTri[keyY][keyX].Shape.IsPointInTriangle4(p))
            {
                return layoutTri[keyY][keyX];
            }
        }

        // See if it in the polygonal electrode area
        if (layout.ContainsKey(keyY) && layout[keyY].ContainsKey(keyX))
        {
            return layout[keyY][keyX];
        }
        return null;
    }
}