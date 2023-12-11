using ExecutionEnv;
using Model;
using System.Text;
using uPLibrary.Networking.M2Mqtt.Messages;

public class MqttClient
{
    private uPLibrary.Networking.M2Mqtt.MqttClient client;
    private string clientId;
    private string clientName;
    private string BrokerAddress = "test.mosquitto.org";

    public MqttClient(string clientName, string brokerHostName)
    {
        if (String.Equals("remote", brokerHostName))
        {
            client = new uPLibrary.Networking.M2Mqtt.MqttClient(BrokerAddress);
        } else if (String.Equals("local", brokerHostName)) {
            client = new uPLibrary.Networking.M2Mqtt.MqttClient("localhost");
        } else
        {
            client = new uPLibrary.Networking.M2Mqtt.MqttClient(brokerHostName);
        }

        this.clientName = clientName;

        // Register a callback-function (we have to implement, see below) which is called by the library when a message was received
        client.MqttMsgPublishReceived += ClientMqttMsgPublishReceived;

        // Use a unique id as client id, each time we start the application
        clientId = Guid.NewGuid().ToString();

        client.Connect(clientId);
    }

    protected void DisConnet()
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

    // This code runs when a message was received
    public void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string receivedMessage = Encoding.UTF8.GetString(e.Message);
        string topic = e.Topic;

        if (topic.Equals("exe/feedback") && receivedMessage.Equals("ok"))
        {
            Part2Tester.executionCompletedFlag = true;
            Video01Tester.executionCompletedFlag = true;
        } 
        else if (topic.Equals("yolo/act"))
        {
            Part2Tester.actualStates = receivedMessage;
            Video01Tester.actualStates = MapYoloInput(receivedMessage);
        } 
        else if (topic.Equals("router/exp"))
        {
            Part2Tester.expectedStates = receivedMessage;
            Video01Tester.expectedStates = receivedMessage;
        } else
        {
            Part1Tester.yolo = receivedMessage;
        }

        // Console.WriteLine($"[Receive][{topic}]:{receivedMessage}");
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
