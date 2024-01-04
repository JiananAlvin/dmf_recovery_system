using Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EndToEndTest
{
    internal class Video01Tester
    {
        internal MqttClientForTest executor = new MqttClientForTest("executor", "local");
        internal MqttClientForTest router = new MqttClientForTest("router", "local");

        internal string YOLO_RESULT_TOPIC = "yolo/act";
        internal string ROUTER_RESULT_TOPIC = "router/exp";
        internal string EXE_FEEDBACK_TOPIC = "exe/feedback";

        internal string PATH_TO_ROUTER = "../../../Video01TesterRouter.json";
        internal string PATH_TO_RESULT_TXT = "../../../recovery_output.txt";

        internal static bool executionCompletedFlag = true;
        internal static bool recoveryCompletedFlag = true;  // Only used for test
        internal static string expectedStates = null;
        internal static string actualStates = null;

        internal int numOfTestCases;

        public void ExecRun()
        {
            // Initialize a corrector
            Corrector corrector = new Corrector();

            // Subscribe expected states and actual states from router and yolo respectively.
            executor.Subscribe(ROUTER_RESULT_TOPIC);
            executor.Subscribe(YOLO_RESULT_TOPIC);
            // Clear content in result.txt
            File.Create(PATH_TO_RESULT_TXT).Close();

            Thread.Sleep(1000);  // Wait for YOLO and router to publish.

            for (int i = 0; i < numOfTestCases; i++)
            {
                Console.WriteLine($"Exec is running for round {i}");

                List<Dictionary<string, HashSet<int>>> electrodesForRecovery;
                do
                {
                    while ((expectedStates is null) || (actualStates is null))
                    {
                        Thread.Sleep(100);
                    }
                    // Correct by given expected states and actual states.
                    electrodesForRecovery = corrector.Run(expectedStates, actualStates, PATH_TO_RESULT_TXT);

                    // If correction result is an empty list (i.e. Actual states match expected states), then give "okay" to executor.
                    if (electrodesForRecovery.Count == 0)
                    {
                        executor.Publish(EXE_FEEDBACK_TOPIC, "ok");
                    }

                    recoveryCompletedFlag = true;
                    // Wait for YOLO and router to publish.
                    Thread.Sleep(1000);
                } while (electrodesForRecovery.Count != 0);
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

            // Publish next expected state whenever executeCompletedFlag = true (i.e. correction was done).
            foreach (JObject obj in routerJsonArray)
            {
                string expectedStates = JsonConvert.SerializeObject(obj["exp"], Formatting.None).ToString();

                while (true)
                {
                    if (executionCompletedFlag)
                    {
                        router.Publish(ROUTER_RESULT_TOPIC, $"{expectedStates}");
                        executionCompletedFlag = false;
                        break;
                    }
                }
            }
        }

        public void Run()
        {
            // Order: 
            // Yolo -> Router -> Execution engine
            new Thread(RouterRun).Start();
            //new Thread(YoloRun).Start();
            ExecRun();
        }
    }
}
