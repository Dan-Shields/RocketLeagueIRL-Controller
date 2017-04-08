using System;
using System.IO.Ports;
using XInputDotNetPure;
using System.Threading;

namespace RobotController
{
    class Program
    {
        static void Main(string[] args)
        {
            bool[] controls = new bool[8];
            Console.ForegroundColor = ConsoleColor.Green;

            SerialPort ser = new SerialPort
            {
                BaudRate = 9600,
                PortName = "COM3"
            };
            ser.Open();
            ser.WriteTimeout = 1000;
            ser.ReadTimeout = 1000;

            // Poll events from joystick
            while (true)
            {
                var gamepad = GamePad.GetState(PlayerIndex.One);

                int turnSpeed = Math.Abs((int)(gamepad.ThumbSticks.Left.X * 255));
                int globalSpeed = Math.Abs((int) ((gamepad.Triggers.Right - gamepad.Triggers.Left) * 255));
                bool left = gamepad.ThumbSticks.Left.X < 0;
                bool right = !left;
                
                bool move = globalSpeed > 10;
                bool headingForward = gamepad.Triggers.Right > gamepad.Triggers.Left;


                controls[0] = move;
                controls[1] = headingForward;
                controls[2] = left;
                controls[3] = right;
                controls[4] = controls[5] = controls[6] = controls[7] = false;

                byte controlByte = 0x00;
                int index = 0;

                // Loop through the array
                foreach (bool b in controls)
                {
                    // if the element is 'true' set the bit at that position
                    if (b)
                        controlByte |= (byte)(1 << (7 - index));

                    index++;
                }

                //Console.WriteLine(controlByte.ToString());
                //Console.WriteLine(turnSpeed.ToString());
                //Console.WriteLine(globalSpeed.ToString());
                
                Byte[] data = {controlByte,/*, (byte) turnSpeed, (byte) globalSpeed,*/ 0x0A};
                //Console.WriteLine("here");
                //ser.Write(data, 0, 2);

                ser.WriteLine("Ayyyyy");
                Console.WriteLine("wassup");

                while (ser.BytesToRead > 0)
                {
                    Console.WriteLine(ser.ReadLine());
                }

                Thread.Sleep(5000);
                break;
            }
            ser.Close();
        }
    }
}
