﻿using ExecutionEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        internal static bool executeCompletedFlag = true;
        internal static string expectedStates = null;
        internal static string actualStates = null;

        internal int numOfTestCases;

        public void ExecRun()
        {
            for (int i = 0; i < numOfTestCases; i++) 
            {
                Thread.Sleep(3000);
                Console.WriteLine($"Exec is running for round {i}");

                // Subscribe expected states and actual states from router and yolo respectively.
                while (expectedStates == null)
                    executor.Subscribe(ROUTER_RESULT_TOPIC);
                
                List<Dictionary<string, HashSet<int>>> electrodesForCalibration;
                do 
                {
                    while (actualStates == null)
                        executor.Subscribe(YOLO_RESULT_TOPIC);

                    // Calibrate by given expected states and actual states.
                    Calibrator calibrator = new Calibrator();
                    electrodesForCalibration = calibrator.Run(expectedStates, actualStates);
                
                    // If calibration result is an empty list (i.e. Actual states match expected states), then give "okay" to executor.
                    if (electrodesForCalibration.Count == 0)
                    {
                        executor.Publish(EXE_FEEDBACK_TOPIC, "ok");
                    }
                    actualStates = null;
                    Thread.Sleep(8000);
                } while (electrodesForCalibration.Count != 0);
                expectedStates = null;
            }
        }

        public void RouterRun()
        {
            // Parse router.json file.
            string routerJsonText = File.ReadAllText(@$"{PATH_TO_ROUTER}");
            JArray routerJsonArray = JArray.Parse(routerJsonText);

            // # of test cases is the size of the JSON array in router.json file.
            numOfTestCases = routerJsonArray.Count; 

            router.Subscribe(EXE_FEEDBACK_TOPIC);

            // Publish next expected state whenever executeCompletedFlag = true (i.e. calibration was done).
            foreach (JObject obj in routerJsonArray)
            {
                string expectedStates = JsonConvert.SerializeObject(obj["exp"], Formatting.None).ToString();
                Thread.Sleep(2000);

                Console.WriteLine("Here is the value of executeCompletedFlag:" + executeCompletedFlag);
                while (true)
                {
                   if (executeCompletedFlag)
                    {
                        router.Publish(ROUTER_RESULT_TOPIC, $"{expectedStates}");
                        executeCompletedFlag = false;
                        break;
                    }
                }
                Thread.Sleep(12000);
            }    
        }

        public void YoloRun()
        {
            // Parse yolo.json file.
            string yoloJsonText = File.ReadAllText(@$"{PATH_TO_YOLO}");
            JArray yoloJsonArray = JArray.Parse(yoloJsonText);

            // Publish each actual state.
            foreach (JObject obj in yoloJsonArray)
            {
                string actualStates = JsonConvert.SerializeObject(obj["act"], Formatting.None).ToString();
                Thread.Sleep(1000);
                yolo.Publish(YOLO_RESULT_TOPIC, $"{actualStates}");
                Thread.Sleep(13000);
            }
        }

        public void Run()
        {
            // Order: 
            // Yolo -> Router -> Execution engine
            new Thread(YoloRun).Start();
            new Thread(RouterRun).Start();
            Thread.Sleep(1000);
            ExecRun();
        }
    }
}
