using System.Text;
using Model;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Engine
{
    public class MqttClient
    {
        private uPLibrary.Networking.M2Mqtt.MqttClient client;
        private string clientId;
        public DateTime previousUpdateTime { get; set; }
        public string previousActualState { get; set; }

        public MqttClient(string brokerHostName)
        {
            previousActualState = "";
            client = new uPLibrary.Networking.M2Mqtt.MqttClient(brokerHostName);
            // Register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += YOLOActualReceived;
            // Use a unique id as client id, each time we start the application
            clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public void Subscribe(string topic)
        {
            // Whole topic
            string finalTopic = $"{topic}";

            // Subscribe to the topic with QoS 2
            client.Subscribe(new string[] { finalTopic }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call

            // Console.WriteLine($"[Subscribe][{topic}]:Success");
        }

        public void Publish(string topic, string content)
        {
            // Whole topic
            string finalTopic = $"{topic}";

            // publish a message with QoS 2
            client.Publish(finalTopic, Encoding.UTF8.GetBytes(content), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
            Console.WriteLine($"[Publish][{topic}]:{content}");
        }

        public void YOLOActualReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string receivedMessage = Encoding.UTF8.GetString(e.Message);
            string topic = e.Topic;

            this.previousActualState = MapYoloInput(receivedMessage);
            this.previousUpdateTime = DateTime.Now;

            Logger.LogReceivedFromYolo($"[{topic}]:{receivedMessage}");
        }

        string MapYoloInput(string receivedMessage)
        {
            Initializer init = new Initializer();
            init.Initilalize();
            Mapper mapper = new Mapper();
            List<List<int>> result = mapper.Map(receivedMessage, init.width, init.height, init.minStep, init.nonTriangleHashMap, init.triangleHashMap);
            return ListOfListsToString(result);
        }

        string ListOfListsToString(List<List<int>> listOfLists)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");
            sb.Append(string.Join(",", listOfLists.Select(innerList => "[" + string.Join(",", innerList) + "]")));
            sb.Append("]");

            return sb.ToString();
        }
    }
}


