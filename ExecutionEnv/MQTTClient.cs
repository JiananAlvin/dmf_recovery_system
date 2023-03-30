using System.Text;
// including the M2Mqtt Library
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class MQTTClient
{

    MqttClient client;
    string clientId;
    string name;
    string BrokerAddress = "test.mosquitto.org";

    public MQTTClient(string name)
    {
        client = new MqttClient(BrokerAddress);

        this.name = name;

        // register a callback-function (we have to implement, see below) which is called by the library when a message was received
        client.MqttMsgPublishReceived += ClientMqttMsgPublishReceived;

        // use a unique id as client id, each time we start the application
        clientId = Guid.NewGuid().ToString();

        client.Connect(clientId);
    }


    protected void DisConnet()
    {
        client.Disconnect();

    }


    public void Subscribe(string topic)
    {
        // whole topic
        string finalTopic = $"/JNAlvin/test/{topic}";

        // subscribe to the topic with QoS 2
        client.Subscribe(new string[] { finalTopic }, new byte[] { 2 });   // we need arrays as parameters because we can subscribe to different topics with one call

        Console.WriteLine($"[{name}][Subscribte][{topic}]:Success");

    }


    public void Publish(string topic, string content)
    {
        // whole topic
        string finalTopic = $"/JNAlvin/test/{topic}";

        // publish a message with QoS 2
        client.Publish(finalTopic, Encoding.UTF8.GetBytes(content), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        Console.WriteLine($"[{name}][Publish][{topic}]:{content}");

    }


    // this code runs when a message was received
    public void ClientMqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string receivedMessage = Encoding.UTF8.GetString(e.Message);

        string topic = e.Topic;


        if (topic.Equals("/JNAlvin/test/exe_feedback") && receivedMessage.Equals("ok"))
        {
            Program.executeCompletedFlag = true;
        }


        Console.WriteLine($"[{name}][Receive][{topic}]:{receivedMessage}");
    }
}
