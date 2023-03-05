using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;


class Program
{
    const String IP = "localhost";
    const int PORT = 1883;

    static void Main(string[] args)
    {
        // create client instance
        MqttClient client = new MqttClient(IP, PORT, false, null, null, MqttSslProtocols.None);

        // register to message received 
        client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
       
        string clientId = Guid.NewGuid().ToString();
        client.Connect(clientId);

        // subscribe to the topic "test" with QoS 2 
        client.Subscribe(new string[] { "yolo" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

        Console.ReadLine();

        // disconnect
        client.Disconnect();
    }

    private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        // handle message received 
        string message = Encoding.UTF8.GetString(e.Message);
        Console.WriteLine("Received message: " + message);
    }
}



