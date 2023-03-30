using System.Threading;
public class Program
{

    static MQTTClient yolo = new MQTTClient("yolo");
    static MQTTClient executor = new MQTTClient("executor");
    static MQTTClient router = new MQTTClient("router");

    public static string YOLO_RESULT_TOPIC = "yolo_result";
    public static string ROUTER_RESULT_TOPIC = "router_result";
    public static string EXE_FEEDBACK_TOPIC = "exe_feedback";

    public static bool executeCompletedFlag = false;



    static void Main(string[] args)
    {

        executor.Subscribe(YOLO_RESULT_TOPIC);

        executor.Subscribe(ROUTER_RESULT_TOPIC);

        router.Subscribe(EXE_FEEDBACK_TOPIC);

        new Thread(YoloPublish).Start();

        new Thread(RouterPublish).Start();

        new Thread(ExecPublish).Start();

    }



    static void ExecPublish()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine($"Exec is runn.ing for round {i}");
            // your code here

            // TODO : 
            Thread.Sleep(1000);
            executor.Publish("exe_feedback", "ok");
        }
    }




    static void RouterPublish()
    {
        while (true)
        {
            router.Publish("router_result", "4 TICK, expected states {electrodeID, tlX,tlY,width,height, Direction}");
            while (true)
            {
                if (executeCompletedFlag)
                {
                    executeCompletedFlag = false;
                    break;
                }
            }
            Thread.Sleep(100);
        }
    }


    static void YoloPublish()
    {
        for (int i = 0; i < 20; i++)
        {
            yolo.Publish("yolo_result", $"yolov5 result {i}");
            Thread.Sleep(500);
        }

    }


}

