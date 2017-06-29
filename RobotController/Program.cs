using System;
using XInputDotNetPure;
using System.Net.Sockets;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RobotController
{
    class Program
    {
        static String host = "192.168.1.114";
        static Int32 port = 26656;
        static TcpClient client;

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            client = new TcpClient(host, port);

            // Poll events from joystick
            while (true)
            {
                var gamepad = GamePad.GetState(PlayerIndex.One);
                var jsonObject = new JObject();

                jsonObject.Add("rt", gamepad.Triggers.Right);
                jsonObject.Add("lt", gamepad.Triggers.Left);
                jsonObject.Add("x", gamepad.ThumbSticks.Left.X);
                jsonObject.Add("b", gamepad.Buttons.B == ButtonState.Pressed);

                SendData(JsonConvert.SerializeObject(jsonObject));

                if (gamepad.Buttons.Back != ButtonState.Pressed)
                {
                    continue;
                }
                break;
            }

            client.Close();
        }

        static void SendData(String message)
        {
            try
            {
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                //stream.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }
    }
}
