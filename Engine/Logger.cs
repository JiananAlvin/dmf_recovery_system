namespace Engine
{
    public class Logger
    {
        public static void LogSendToDMF(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[Send To DMF]:" + message + "\n");
            Console.ResetColor();
        }

        public static void LogReceivedFromYolo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("[Received from yolo]:" + message + "\n");
            Console.ResetColor();
        }
    }
}