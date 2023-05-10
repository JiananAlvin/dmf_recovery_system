import paho.mqtt.client as mqtt
import time

IP = "localhost"
PORT = 1883


def publisher(topic, payload):

    # Set up the client
    client = mqtt.Client()

    # Connect to the broker
    client.connect(IP, PORT)

    # Ensure that the MQTT client has enough time to complete its connection process before executing other commands.
    time.sleep(0.1)

    # Publish a message
    client.publish(topic, payload)

    # Disconnect from the broker
    client.disconnect()


# def main():
#     publisher()
#
#
# if __name__ == "__main__":
#     main()
