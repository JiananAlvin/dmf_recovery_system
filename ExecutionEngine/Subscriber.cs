using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


class Subscriber
{
    private MqttClient client;
    private string receivedMessage;

    public Subscriber(string ip, int port)
    {
        // Create client instance
        this.client = new MqttClient(ip, port, false, null, null, MqttSslProtocols.None);

        // Register to message received 
        this.client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

        string clientId = Guid.NewGuid().ToString();
        this.client.Connect(clientId);
    }

    public void Subscribe(string topic)
    { 
        // Subscribe to the topic "test" with QoS 2 
        this.client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        // Console.ReadLine();

        // Disconnect
        // client.Disconnect();
    }

    private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        // Handle message received 
        this.receivedMessage = Encoding.UTF8.GetString(e.Message);
        // Console.WriteLine("Received message: " + receivedMessage);
    }

    public string GetReceivedMessage()
    {
        return this.receivedMessage;
    }
}



