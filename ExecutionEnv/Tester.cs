using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionEnv
{
    internal class Tester
    {
        internal MQTTClient yolo = new MQTTClient("yolo");
        internal MQTTClient executor = new MQTTClient("executor");
        internal MQTTClient router = new MQTTClient("router");

        internal string YOLO_RESULT_TOPIC = "yolo/act";
        internal string ROUTER_RESULT_TOPIC = "router/exp";
        internal string EXE_FEEDBACK_TOPIC = "exe/feedback";

        internal string PATH_TO_YOLO = "../../../yolo.json";
        internal string PATH_TO_ROUTER = "../../../router.json";



        internal static bool executeCompletedFlag = false;

        public Tester() 
        {
            // routerJsonArray = JArray.Parse(routerJsonText);
            
        }

        public void ExecPublish()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Exec is runn.ing for round {i}");
                // your code here

                // TODO : 
                Thread.Sleep(1000);
                executor.Publish("exe/feedback", "ok");
            }
        }

        public void RouterPublish()
        {
            string routerJsonText = File.ReadAllText(@$"{PATH_TO_ROUTER}");
            JArray routerJsonArray = JArray.Parse(routerJsonText);

            while (true)
            {
                router.Publish("router/exp", "4 TICK, expected states {electrodeID, tlX,tlY,width,height, Direction}");
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

        public void YoloPublish()
        {
            string yoloJsonText = File.ReadAllText(@$"{PATH_TO_YOLO}");
            JArray yoloJsonArray = JArray.Parse(yoloJsonText);
            
            // Loop through each object in the array and print the "act" property
            foreach (JObject obj in yoloJsonArray)
            {
                string actualStates = obj["act"].ToString();
                yolo.Publish("yolo/act", $"{actualStates}");
                Thread.Sleep(500);
            }
        }

        public void Run()
        {
            executor.Subscribe(YOLO_RESULT_TOPIC);
            executor.Subscribe(ROUTER_RESULT_TOPIC);
            router.Subscribe(EXE_FEEDBACK_TOPIC);

            new Thread(YoloPublish).Start();
            new Thread(RouterPublish).Start();
            new Thread(ExecPublish).Start();
        }
    }
}
