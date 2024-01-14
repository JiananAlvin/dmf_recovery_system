using System.Runtime.CompilerServices;
using System.Text;
using System.IO.Ports;
[assembly: InternalsVisibleTo("Test")]

namespace Engine
{
    class SerialManager
    {
        public SerialPort port;
        public bool failure = false;

        public SerialManager(string serialPortName, int serialPortSpeed)
        {
            // Create a new SerialPort object with default settings.
            port = new SerialPort();
            // Allow the user to set the appropriate properties.
            port.PortName = serialPortName;
            port.BaudRate = serialPortSpeed;
            port.Parity = Parity.None;
            port.DataBits = 8;
            port.StopBits = StopBits.One;
            port.Handshake = Handshake.None;
            // Set the read/write timeouts
            port.ReadTimeout = 500;
            port.WriteTimeout = 500;
            port.Encoding = Encoding.GetEncoding(28591);
            //port.ReadBufferSize = 128;
        }

        public bool OpenPort()
        {
            try
            {
                port.Open();
                Console.WriteLine("PortOpen");
                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("PortCouldNotOpen");
                return false;
            }
        }

        public void ClosePort()
        {
            Console.WriteLine("PortClosed");
            port.Close();
        }

        public List<char> Read()
        {
            try
            {
                int size = port.BytesToRead;
                char[] input = new char[size];
                port.Read(input, 0, size);
                return input.ToList();
            }
            catch (Exception)
            {
                Console.WriteLine("Serial access failed");
                failure = true;
                return new List<char>();

            }
        }

        public void Write(string message)
        {
            try
            {
                char[] output = message.ToCharArray();
/*                
                foreach (var item in output)
                {
                    Console.WriteLine("-> " + Convert.ToInt32(item));
                }*/
                
                port.Write(output, 0, output.Length);
            }
            catch (Exception)
            {
                Console.WriteLine("Serial write access failed");
                failure = true;
            }
        }

        public static List<string> ListSerialPorts()
        {

            return SerialPort.GetPortNames().ToList();

        }


    }
}
