namespace Engine
{

    class Element
    {
        public class Droplet
        {
            public string name { get; set; }
            public int ID { get; set; }
            public int centerX { get; set; }
            public int centerY { get; set; }
            public int sizeX { get; set; }
            public int sizeY { get; set; }
        }

        public class Electrode
        {
            public enum Shape { RECTANGLE=0, POLIGON=1 };

            public string name { get; set; }
            public int ID { get; set; }
            public int electrodeID { get; set; } = -1;
            public int driverID { get; set; } = -1;
            public Shape shape { get; set; }
            public int positionX { get; set; }
            public int positionY { get; set; }
            public int sizeX { get; set; }
            public int sizeY { get; set; }
            public bool status { get; set; }
            public List<int[]> corners { get; set; }
        }

        public class Heater
        {
            public enum Shape { RECTANGLE=0, POLIGON=1 };

            public string name { get; set; }
            public int ID { get; set; }
            public Shape shape { get; set; }
            public int positionX { get; set; }
            public int positionY { get; set; }
            public int sizeX { get; set; }
            public int sizeY { get; set; }
            public double actualTemperature { get; set; }
            public double desiredTemperature { get; set; }
            public bool status { get; set; }
            public double nextDesiredTemperature { get; set; }
            public bool nextStatus { get; set; }
            public List<int[]> corners { get; set; }
        }

        public class RGBSensor
        {
            public enum Shape { CIRCLE=0 };

            public string name { get; set; }
            public int ID { get; set; }
            public Shape shape { get; set; }
            public int positionX { get; set; }
            public int positionY { get; set; }
            public int sizeX { get; set; }
            public int sizeY { get; set; }
            public int valueRed { get; set; }
            public int valueGreen { get; set; }
            public int valueBlue { get; set; }
        }

        public class Nunchuck
        {
            public string name { get; set; }
            public int ID { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public bool C { get; set; }
            public bool Z { get; set; }
        }

        public class Winner
        {
            public string name { get; set; }
            public int ID { get; set; }
            public int winner { get; set; }
        }
    }
}
