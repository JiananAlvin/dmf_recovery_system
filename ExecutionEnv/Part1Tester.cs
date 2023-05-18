using ExecutionEngine;

namespace ExecutionEnv
{
    public class Part1Tester
    {
        internal string TOPIC = "yolo/det";
        public static int width;
        public static int height;
        public static int minStep;
        public static double tolerance = 0;  // This one should be user input.
        public static int sizeOfSquareEl = 20; // TODO: This should be read from JSON somehow.
        public static Dictionary<int, Dictionary<int, Electrode>> layout;
        public static Dictionary<int, Dictionary<int, Electrode>> layoutTri;
        public static String yolo = null;

        public void Run() 
        {
            // Init two maps in terms of input JSON file
            Initializer init = new Initializer();
            init.Initilalize();
            width = init.width;
            height = init.height;
            minStep = init.minStep;
            layout = init.layout;
            layoutTri = init.layoutTri;

            // Subscribe YOLO output
            MqttClient s = new MqttClient("yolo", "local");
            s.Subscribe(TOPIC);
            // string yolo = "{ 'img_dimension': [671, 320], 'droplet_info': [[632.0, 239.0, 10, 12], [298.0, 353.0, 28, 30], [581.0, 310.0, 30, 32]]}";

            // Map
            Mapper mapper = new Mapper();
            
            // Wait for the first message
            while (yolo == null)
            {

            }

            int frameCounter = -1;
            while (true) {
                Console.WriteLine("Frame " + ++frameCounter + ":");

                int dropletCounter = -1;
                while (yolo is not null)
                {
                    // Console.WriteLine(yolo.ToString());
                    // yolo is actualS
                    List<List<int>> result = mapper.Map(yolo, width, height, minStep, layout, layoutTri); //TODO

                    // Print results in the terminal
                    Console.WriteLine("  Droplet " + ++dropletCounter + ":");
                    for (int i = 0; i < result.Count; i++)
                    {
                        Console.Write("    electrodeId': " + result[i][0] + "; ");
                        Console.Write("xTopLeft': "        + result[i][1] + "; ");
                        Console.Write("yTopLeft': "        + result[i][2] + "; ");
                        Console.Write("width': "           + result[i][3] + "; ");
                        Console.Write("height': "          + result[i][4] + "; ");
                        Console.Write("xOffset': "         + result[i][5] + "; ");
                        Console.WriteLine("yOffset': "     + result[i][6]);
                    }
                    // Empty message from "yolo/det"
                    yolo = null;
                }
            }
        }
    }
}
