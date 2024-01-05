using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Engine
{
       
    class Platform
    {
        public int sizeX { get; set; }
        public int sizeY { get; set; }

        public List<Element.Droplet> droplets { get; set; } = new List<Element.Droplet>();
        public List<Element.Electrode> electrodes { get; set; } = new List<Element.Electrode>();
        public List<Element.Heater> heaters { get; set; } = new List<Element.Heater>();
        public List<Element.RGBSensor> RGBSensors { get; set; } = new List<Element.RGBSensor>();
        public List<Element.Nunchuck> nunchucks { get; set; } = new List<Element.Nunchuck>();
        public List<Element.Winner> winners { get; set; } = new List<Element.Winner>();
        public Dictionary<string,int> data { get; set; } = new Dictionary<string,int>();

        public List<string> commands { get; set; } = new List<string>();

    }

    static class PlatformUtilities
    {
        public static void SavePlatform(Platform platform)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string jsonString = JsonSerializer.Serialize<Platform>(platform, options);
            //Console.WriteLine(jsonString);
            // File.WriteAllText("./../../platform640.json", jsonString);
            // ..\..\..\..\Cases\Case0\platform640v2.json
            File.WriteAllText("../../../../Cases/Case0/platform640v2.json", jsonString);
        }

        public static Platform LoadPlatform()
        {
            // string jsonString = File.ReadAllText("./../../platform640.json");
            string jsonString = File.ReadAllText("../../../../Cases/Case0/platform640v2.json");
            Platform platform = JsonSerializer.Deserialize<Platform>(jsonString);
            return platform;
        }

        public static readonly int[,] Top = new int[,]
       {/*       40, 39, 38,    37, 36, 35, 34, 33, 32, 31, 30, 29, 28, 27, 26, 25, 24, 23, 22,     21,        20,      29, 18, 17, 16, 15, 14, 13, 12, 11, 10,  9,  8,  7,  6,  5,  4,     3,  2,  1        */
         /*K*/  {330,340,350,   360,370,380,262,272,282,292,302,312,194,204,214,224,234,244,254,     0 ,        0 ,     131,141,151,161,171,181,191, 73, 83, 93,103,113,123,  5, 15, 25,    35, 45, 55},   /*K*/
         /*J*/  {329,339,349,   359,369,379,261,271,281,291,301,311,193,203,213,223,233,243,253,     0 ,        0 ,     132,142,152,162,172,182,192, 74, 84, 94,104,114,124,  6, 16, 26,    36, 46, 56},   /*J*/
         /*H*/  {328,338,348,   358,368,378,260,270,280,290,300,310,320,202,212,222,232,242,252,     0 ,        0 ,     133,143,153,163,173,183, 65, 75, 85, 95,105,115,125,  7, 17, 27,    37, 47, 57},   /*H*/
         /*G*/  {327,337,347,   357,367,377,259,269,279,289,299,309,319,201,211,221,231,241,251,     0 ,        0 ,     134,144,154,164,174,184, 66, 76, 86, 96,106,116,126,  8, 18, 28,    38, 48, 58},   /*G*/
         /*F*/  {326,336,346,   356,366,376,258,268,278,288,298,308,318,200,210,220,230,240,250,     0 ,        0 ,     135,145,155,165,175,185, 67, 77, 87, 97,107,117,127,  9, 19, 29,    39, 49, 59},   /*F*/
         /*E*/  {325,335,345,   355,365,375,257,267,277,287,297,307,317,199,209,219,229,239,249,     0 ,        0 ,     136,146,156,166,176,186, 68, 78, 88, 98,108,118,128, 10, 20, 30,    40, 50, 60},   /*E*/
         /*D*/  {324,334,344,   354,364,374,384,266,276,286,296,306,316,198,208,218,228,238,248,     0 ,        0 ,     137,147,157,167,177,187, 69, 79, 89, 99,109,119,  1, 11, 21, 31,    41, 51, 61},   /*D*/
         /*C*/  {323,333,343,   353,363,373,383,265,275,285,295,305,315,197,207,217,227,237,247,     0 ,        0 ,     138,148,158,168,178,188, 70, 80, 90,100,110,120,  2, 12, 22, 32,    42, 52, 62},   /*C*/
         /*B*/  {322,332,342,   352,362,372,382,264,274,284,294,304,314,196,206,216,226,236,246,    256,       129,     139,149,159,169,179,189, 71, 81, 91,101,111,121,  3, 13, 23, 33,    43, 53, 63},   /*B*/
         /*A*/  {321,331,341,   351,361,371,381,263,273,283,293,303,313,195,205,215,225,235,245,    255,       130,     140,150,160,170,180,190, 72, 82, 92,102,112,122,  4, 14, 24, 34,    44, 54, 64}    /*A*/
       };

        /// <summary>
        /// This method generates the 32x20 platform
        /// </summary>
        /// <returns></returns>
        public static Platform Generate32x20()
        {
            Platform platform = new Platform();

            const int offsetX = 0;
            const int offsetY = 0;
            //const int electrodePadding = 0; // Space between electrodes
            const int electrodeSize = 20; // X and Y size of an electrode
            const int dropletSize = 40; // X and Y size of an electrode
            const int arrayXCount = 32;
            const int arrayYCount = 20;
            const int reservXOffset = 3 * electrodeSize + (int) (2.5 * electrodeSize);

            platform.sizeX = reservXOffset + electrodeSize * arrayXCount + reservXOffset;
            platform.sizeY = electrodeSize * arrayYCount;

            //Generate test droplet
            platform.droplets.Add(new Element.Droplet());
            platform.droplets[0].name = "drop1";
            platform.droplets[0].ID = 0;
            platform.droplets[0].centerX = offsetX + 1 * electrodeSize + reservXOffset + electrodeSize / 2;
            platform.droplets[0].centerY = offsetY + 1 * electrodeSize + electrodeSize / 2;
            platform.droplets[0].sizeX = dropletSize;
            platform.droplets[0].sizeY = dropletSize;

            // Generate the Winner
            platform.winners.Add(new Element.Winner());
            platform.winners[0].name = "winner";
            platform.winners[0].ID = 0;
            platform.winners[0].winner = 0;
            
            // Generate the Nunchucks
            platform.nunchucks.Add(new Element.Nunchuck());
            platform.nunchucks[0].name = "NC1";
            platform.nunchucks[0].ID = 1;
            platform.nunchucks[0].X = 0;
            platform.nunchucks[0].Y = 0;
            platform.nunchucks[0].C = false;
            platform.nunchucks[0].Z = false;
            platform.nunchucks.Add(new Element.Nunchuck());
            platform.nunchucks[1].name = "NC2";
            platform.nunchucks[1].ID = 2;
            platform.nunchucks[1].X = 0;
            platform.nunchucks[1].Y = 0;
            platform.nunchucks[1].C = false;
            platform.nunchucks[1].Z = false;

            // Generate the RGB sensors
            platform.RGBSensors.Add(new Element.RGBSensor());
            platform.RGBSensors[0].name = "RGBsensor";
            platform.RGBSensors[0].ID = 1;
            platform.RGBSensors[0].shape = Element.RGBSensor.Shape.CIRCLE;
            platform.RGBSensors[0].positionX = offsetX + (int)(8.5 * electrodeSize + reservXOffset);
            platform.RGBSensors[0].positionY = offsetY + (int)(8.5 * electrodeSize);
            platform.RGBSensors[0].sizeX = 2 * electrodeSize;
            platform.RGBSensors[0].sizeY = 2 * electrodeSize;
            platform.RGBSensors[0].valueRed = 0;
            platform.RGBSensors[0].valueGreen = 0;
            platform.RGBSensors[0].valueBlue = 0;

            // Generate the heaters
            platform.heaters.Add(new Element.Heater());

            platform.heaters[0].name = "heat2";
            platform.heaters[0].ID = 2;
            platform.heaters[0].shape = Element.Heater.Shape.RECTANGLE;
            platform.heaters[0].positionX = offsetX + 8 * electrodeSize + reservXOffset;
            platform.heaters[0].positionY = offsetY;
            platform.heaters[0].sizeX = 4 * electrodeSize;
            platform.heaters[0].sizeY = 20 * electrodeSize;
            platform.heaters[0].actualTemperature = 0.0;
            platform.heaters[0].desiredTemperature = 0.0;
            platform.heaters[0].status = false;
            platform.heaters[0].nextDesiredTemperature = 0.0;
            platform.heaters[0].nextStatus = false;


            int index = 0;
            // Generate the regular array            
            for (int y = 0; y < arrayYCount; y++)
            {
                for (int x = 0; x < arrayXCount; x++)
                {
                    index = x + arrayXCount * y;
                    platform.electrodes.Add(new Element.Electrode());
                    platform.electrodes[index].ID = index+1;
                    platform.electrodes[index].name = "arrel" + (index+1).ToString();
                    platform.electrodes[index].positionX = offsetX + x * electrodeSize + reservXOffset;
                    platform.electrodes[index].positionY = offsetY + y * electrodeSize; 
                    platform.electrodes[index].sizeX = electrodeSize;
                    platform.electrodes[index].sizeY = electrodeSize;
                    platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
                    platform.electrodes[index].status = false;
                }
            }

            //Setting driver and electrode ID according to chip mapping
            int el_cnt = 0;
            for (int row = 0; row < 20; row++)
            {
                for (int col = 0; col < 39; col++)
                {
                    if (col != 0 && col != 1 && col != 2 && col != 19 && col != 20 && col != 37 && col != 38 && col != 39)
                    {

                        if (row < 10)
                        {
                            //Mapping[el_cnt] = ChipMappingTc008.Top[row, col];
                            platform.electrodes[el_cnt].driverID = 0;
                            platform.electrodes[el_cnt].electrodeID = Top[row, col];

                        }
                        else
                        {
                            //Mapping[el_cnt] = ChipMappingTc008.Top[19-row, 39-col];
                            platform.electrodes[el_cnt].driverID = 1;
                            platform.electrodes[el_cnt].electrodeID = Top[19 - row, 39 - col];
                        }
                        el_cnt++;
                    }
                }
            }

            // Generate RES1
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res1el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX;
            platform.electrodes[index].positionY = offsetY + 1 * electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 3*electrodeSize, 0 },
                new int[2] { 3 * electrodeSize, 1 * electrodeSize },
                new int[2] { (int)(2.5 * electrodeSize), 1*electrodeSize },
                new int[2] { (int)(2.5 * electrodeSize), 2*electrodeSize },
                new int[2] { 3 * electrodeSize, 2*electrodeSize },
                new int[2] { 3 * electrodeSize, 3 * electrodeSize },
                new int[2] { 0, 3 * electrodeSize } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res1el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 1 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res1el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(2.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 2 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res1el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 2 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res1el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(4.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 2 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res1el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 3 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            // Generate RES2
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res2el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(0 * electrodeSize); ;
            platform.electrodes[index].positionY = offsetY + 5 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 3*electrodeSize, 0 },
                new int[2] { 3 * electrodeSize, 1 * electrodeSize },
                new int[2] { (int)(2 * electrodeSize), 1*electrodeSize },
                new int[2] { (int)(2 * electrodeSize), 2*(electrodeSize) },
                new int[2] { 3 * electrodeSize, 2*(electrodeSize) },
                new int[2] { 3 * electrodeSize, 3 * (electrodeSize )},
                new int[2] { 0, 3 * (electrodeSize)} };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res2el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 5 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res2el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(2 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 6 * (electrodeSize);
            platform.electrodes[index].sizeX = (int) (1.5*electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res2el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 6 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res2el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(4.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 6 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res2el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 7 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            // Generate RES3
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX;
            platform.electrodes[index].positionY = offsetY + (int)(8.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 3*electrodeSize, 0 },
                new int[2] { 3 * electrodeSize, 1 * electrodeSize },
                new int[2] { (int)(2.5 * electrodeSize), 1*electrodeSize },
                new int[2] { (int)(2.5 * electrodeSize), 2*electrodeSize },
                new int[2] { 3 * electrodeSize, 2*electrodeSize },
                new int[2] { 3 * electrodeSize, 3 * electrodeSize },
                new int[2] { 0, 3 * electrodeSize } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(8.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(4.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(9 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5*electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(2.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(9.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(9.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(4.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(9.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(2.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(10 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(10 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(4.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(10 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(10.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res3el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(4.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + (int)(10.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            // Generate RES4
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res4el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(0 * electrodeSize); ;
            platform.electrodes[index].positionY = offsetY + 12 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 3*electrodeSize, 0 },
                new int[2] { (int)(2 * electrodeSize), 1*electrodeSize },
                new int[2] { (int)(2 * electrodeSize), 2*(electrodeSize) },
                new int[2] { 3 * electrodeSize, 3 * (electrodeSize)},
                new int[2] { 0, 3 * (electrodeSize) } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res4el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize); ;
            platform.electrodes[index].positionY = offsetY + 12 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 0, electrodeSize },
                new int[2] { -electrodeSize, electrodeSize } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res4el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 12 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res4el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(2 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 13 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(1.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res4el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 13 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res4el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(4.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 13 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res4el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize); ;
            platform.electrodes[index].positionY = offsetY + 14 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 0, electrodeSize },
                new int[2] { -electrodeSize, 0 } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res4el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 14 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            // Generate RES5
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(0 * electrodeSize); ;
            platform.electrodes[index].positionY = offsetY + 16 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 3*electrodeSize, 0 },
                new int[2] { (int)(2 * electrodeSize), 1*electrodeSize },
                new int[2] { (int)(2 * electrodeSize), 2*(electrodeSize) },
                new int[2] { 3 * electrodeSize, 3 * (electrodeSize)},
                new int[2] { 0, 3 * (electrodeSize) } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize); ;
            platform.electrodes[index].positionY = offsetY + 16 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 0, electrodeSize },
                new int[2] { -electrodeSize, electrodeSize } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 16 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(2 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(2.5 * electrodeSize) ;
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(4 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(4.5 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize); ;
            platform.electrodes[index].positionY = offsetY + 18 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 0, electrodeSize },
                new int[2] { -electrodeSize, 0 } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res5el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(3 * electrodeSize);
            platform.electrodes[index].positionY = offsetY + 18 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            // Generate RES6
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res6el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + 2*reservXOffset + arrayXCount*electrodeSize;
            platform.electrodes[index].positionY = offsetY + 1 * electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { -3*electrodeSize, 0 },
                new int[2] { -3 * electrodeSize, 1 * electrodeSize },
                new int[2] { -(int)(2.5 * electrodeSize), 1*electrodeSize },
                new int[2] { -(int)(2.5 * electrodeSize), 2*electrodeSize },
                new int[2] { -3 * electrodeSize, 2*electrodeSize },
                new int[2] { -3 * electrodeSize, 3 * electrodeSize },
                new int[2] { 0, 3 * electrodeSize } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res6el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 3 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res6el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 2 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res6el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 2 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res6el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(5.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 2 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res6el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 1 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            // Generate RES7
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res7el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + (int)(0 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 5 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { -3*electrodeSize, 0 },
                new int[2] { -3 * electrodeSize, 1 * electrodeSize },
                new int[2] { -(int)(2 * electrodeSize), 1*electrodeSize },
                new int[2] { -(int)(2 * electrodeSize), 2*(electrodeSize) },
                new int[2] { -3 * electrodeSize, 2*(electrodeSize) },
                new int[2] { -3 * electrodeSize, 3 * (electrodeSize )},
                new int[2] { 0, 3 * (electrodeSize)} };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res7el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 7 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res7el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 6 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(1.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res7el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 6 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res7el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(5.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 6 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res7el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 5 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            // Generate RES8
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(8.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { -3*electrodeSize, 0 },
                new int[2] { -3 * electrodeSize, 1 * electrodeSize },
                new int[2] { -(int)(2.5 * electrodeSize), 1*electrodeSize },
                new int[2] { -(int)(2.5 * electrodeSize), 2*electrodeSize },
                new int[2] { -3 * electrodeSize, 2*electrodeSize },
                new int[2] { -3 * electrodeSize, 3 * electrodeSize },
                new int[2] { 0, 3 * electrodeSize } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(10.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(5.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(10.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(10 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(10 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(5.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(10 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(9.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(9.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(5.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(9.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(8.5 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res8el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(5.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + (int)(9 * electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = (int)(0.5 * electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            // Generate RES4
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res9el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 12 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { -3*electrodeSize, 0 },
                new int[2] { -(int)(2 * electrodeSize), 1*electrodeSize },
                new int[2] { -(int)(2 * electrodeSize), 2*(electrodeSize) },
                new int[2] { -3 * electrodeSize, 3 * (electrodeSize)},
                new int[2] { 0, 3 * (electrodeSize) } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res9el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 14 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 0, electrodeSize },
                new int[2] { electrodeSize, 0 } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res9el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 14 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res9el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 13 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(1.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res9el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 13 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res9el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(5.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 13 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res9el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 12 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 0, electrodeSize },
                new int[2] { electrodeSize, electrodeSize } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res9el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 12 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            // Generate RES10
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 16 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { -3*electrodeSize, 0 },
                new int[2] { -(int)(2 * electrodeSize), 1*electrodeSize },
                new int[2] { -(int)(2 * electrodeSize), 2*(electrodeSize) },
                new int[2] { -3 * electrodeSize, 3 * (electrodeSize)},
                new int[2] { 0, 3 * (electrodeSize) } };

            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 18 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 0, electrodeSize },
                new int[2] { electrodeSize, 0 } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 18 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(2.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = (int)(0.5 * electrodeSize);
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(5.5 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 17 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(3 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 16 * (electrodeSize);
            platform.electrodes[index].shape = Element.Electrode.Shape.POLIGON;
            platform.electrodes[index].status = false;
            platform.electrodes[index].corners = new List<int[]>() {
                new int[2] { 0, 0 },
                new int[2] { 0, electrodeSize },
                new int[2] { electrodeSize, electrodeSize } };
            index++;
            platform.electrodes.Add(new Element.Electrode());
            platform.electrodes[index].ID = index + 1;
            platform.electrodes[index].name = "res10el" + (index + 1).ToString();
            platform.electrodes[index].positionX = offsetX - (int)(4 * electrodeSize) + 2 * reservXOffset + arrayXCount * electrodeSize;
            platform.electrodes[index].positionY = offsetY + 16 * (electrodeSize);
            platform.electrodes[index].sizeX = electrodeSize;
            platform.electrodes[index].sizeY = electrodeSize;
            platform.electrodes[index].shape = Element.Electrode.Shape.RECTANGLE;
            platform.electrodes[index].status = false;

            return platform;
        }

    }
}
