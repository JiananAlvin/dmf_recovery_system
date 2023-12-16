using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;

public class Mapper
{
    // Maps image-based droplets' imformation to simulator-based droplets' information
    // the out put format is [[eltrodeID, xTopLeft, yTopLeft, width, height, xoffset, yoffset], [], [], ...]
    public List<List<int>> Map(string receivedMessage, int chipWidth, int chipHeight, int minSize, Dictionary<int, Dictionary<int, Electrode>> nonTriangleHashMap, Dictionary<int, Dictionary<int, Electrode>> triangleHashMap)
    {
        // yolo = "{ 'img_dimension': [671, 320], 'droplet_info': [[632.0, 239.0, 10, 12], [298.0, 353.0, 28, 30], [581.0, 310.0, 30, 32]]}";
        dynamic json = JsonConvert.DeserializeObject(receivedMessage);
        int picWidth = (int)json.img_dimension[0];
        int picHeight = (int)json.img_dimension[1];
        float widthRatio = (float)chipWidth / picWidth;
        float heightRatio = (float)chipHeight / picHeight;

        List<List<int>> output = new List<List<int>>();
        foreach (JArray dropletInfo in json.droplet_info)
        {
            List<int> info = new List<int>();
            int xTopLeft = (int)Math.Round((int)dropletInfo[0] * widthRatio);
            int yTopLeft = (int)Math.Round((int)dropletInfo[1] * heightRatio);
            int dropletWidth = (int)Math.Round((int)dropletInfo[2] * widthRatio);
            int dropletHeight = (int)Math.Round((int)dropletInfo[3] * heightRatio);

            // TODO:  Remove offset if the offset is too small
            if (xTopLeft >= 110 && xTopLeft <= 750 && yTopLeft >= 0 && yTopLeft <= 400)
            {
                int deltaX = (xTopLeft - 110) % Initializer.sizeOfSquareEl;
                int deltaY = yTopLeft % Initializer.sizeOfSquareEl;
                if (deltaX >= Initializer.sizeOfSquareEl / 2)
                {
                    xTopLeft += (Initializer.sizeOfSquareEl - deltaX);
                    dropletWidth -= (Initializer.sizeOfSquareEl - deltaX);
                }
                if ((xTopLeft + dropletWidth - 110) % 20 < Initializer.sizeOfSquareEl / 2)
                {
                    dropletWidth -= ((xTopLeft + dropletWidth - 110) % 20);
                }

                if (deltaY >= Initializer.sizeOfSquareEl / 2)
                {
                    yTopLeft += (Initializer.sizeOfSquareEl - deltaY);
                    dropletHeight -= (Initializer.sizeOfSquareEl - deltaY);
                }
                if ((yTopLeft + dropletHeight) % 20 < Initializer.sizeOfSquareEl / 2)
                {
                    dropletHeight -= ((yTopLeft + dropletHeight) % 20);
                }
            }

            // Gets the electrode object
            Electrode el = GetElectrode(xTopLeft, yTopLeft, minSize, nonTriangleHashMap, triangleHashMap);
            int elNo = el.Id;
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

    public Electrode GetElectrode(int xPixel, int yPixel, int minSize, Dictionary<int, Dictionary<int, Electrode>> nonTriangleHashMap, Dictionary<int, Dictionary<int, Electrode>> triangleHashMap)
    {
        int keyX = (int)(xPixel / minSize) * minSize;
        int keyY = (int)(yPixel / minSize) * minSize;

        if (triangleHashMap.ContainsKey(keyY) && triangleHashMap[keyY].ContainsKey(keyX))
        {
            // See if it in the triangular electrode area
            Point p = new Point(xPixel, yPixel);
            if (triangleHashMap[keyY][keyX].Triangle.IsPointInTriangle(p))
            {
                return triangleHashMap[keyY][keyX];
            }
        }

        // See if it in the polygonal electrode area
        if (nonTriangleHashMap.ContainsKey(keyY) && nonTriangleHashMap[keyY].ContainsKey(keyX))
        {
            return nonTriangleHashMap[keyY][keyX];
        }
        return null;
    }
}