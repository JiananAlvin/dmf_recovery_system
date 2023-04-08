using ExecutionEngine;
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
        internal static string expectedStates = null;
        internal static string actualStates = null;

        public void ExecPublish()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Exec is running for round {i}");
                while (expectedStates is null)
                    executor.Subscribe(ROUTER_RESULT_TOPIC);
                while (actualStates is null)
                    executor.Subscribe(YOLO_RESULT_TOPIC);
                // your code here
                /*                executor.Subscribe(ROUTER_RESULT_TOPIC);
                                string expectedStates = null;
                                while (expectedStates is null)
                                {
                                    expectedStates = executor.GetReceivedMessage();
                                    Console.WriteLine("---------------------------");
                                    Console.WriteLine(expectedStates);
                                    Console.WriteLine("---------------------------");
                                }

                                Thread.Sleep(10000);
                                executor.Subscribe(YOLO_RESULT_TOPIC);
                                string actualStates = null;
                                while (actualStates is null)
                                {
                                    actualStates = executor.GetReceivedMessage();
                                    Console.WriteLine("============================");
                                    Console.WriteLine(actualStates);
                                    Console.WriteLine("============================");
                                }*/

                Calibrator calibrator = new Calibrator();
                calibrator.Run(expectedStates, actualStates);
                // TODO : 
                Thread.Sleep(10000);
                executor.Publish(EXE_FEEDBACK_TOPIC, "ok");
            }
        }

        public void RouterPublish()
        {
            string routerJsonText = File.ReadAllText(@$"{PATH_TO_ROUTER}");
            JArray routerJsonArray = JArray.Parse(routerJsonText);

            foreach (JObject obj in routerJsonArray)
            {
                string expectedStates = obj["exp"].ToString();
                while (true)
                {
                    router.Publish(ROUTER_RESULT_TOPIC, $"{expectedStates}");
                    while (true)
                    {
                        if (executeCompletedFlag)
                        {
                            executeCompletedFlag = false;
                            break;
                        }
                    }
                    Thread.Sleep(10000);
                }
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
                yolo.Publish(YOLO_RESULT_TOPIC, $"{actualStates}");
                Thread.Sleep(10000);
            }
        }

        public void Run()
        {
            // executor.Subscribe(YOLO_RESULT_TOPIC);
            // executor.Subscribe(ROUTER_RESULT_TOPIC);
            // router.Subscribe(EXE_FEEDBACK_TOPIC);

            new Thread(YoloPublish).Start();
            new Thread(RouterPublish).Start();
            // new Thread(ExecPublish).Start();
            ExecPublish();
        }
    }
}
