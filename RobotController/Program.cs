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

            //SerialPort ser = new SerialPort();
            //ser.BaudRate = 9600;
            //ser.PortName = "COM7";
            //ser.Open();

           
            // Poll events from joystick
            while (true)
            {
                var gamepad = GamePad.GetState(PlayerIndex.One);

                int turnSpeed = (int) (gamepad.ThumbSticks.Left.X * 255);
                bool left = false;
                bool right = false;
                bool headingForward = true;
                bool move = false;

                //Correct turnSpeed
                if (turnSpeed < 0)
                {
                    turnSpeed++;
                    turnSpeed *= -1;

                    if (turnSpeed > 30)
                    {
                        left = true;
                    }
                } else if (turnSpeed > 30)
                {
                    right = true;
                }


                int globalSpeed = (int) ((gamepad.Triggers.Right - gamepad.Triggers.Left) * 255);

                if (globalSpeed < 0)
                {
                    headingForward = false;
                    globalSpeed *= -1;
                }

                if (turnSpeed > 60 || globalSpeed > 0)
                {
                    move = true;
                }

                controls[0] = move;
                controls[1] = headingForward;
                controls[2] = left;
                controls[3] = right;
                controls[4] = controls[5] = controls[6] = controls[7] = false;

                byte controlByte = 0;
                int index = 0;

                // Loop through the array
                foreach (bool b in controls)
                {
                    // if the element is 'true' set the bit at that position
                    if (b)
                        controlByte |= (byte)(1 << (7 - index));

                    index++;
                }

                char controlChar = Convert.ToChar(controlByte);

                int controlInt = Convert.ToInt16(controlChar);

                Console.WriteLine(controlInt.ToString());
                Console.WriteLine(turnSpeed.ToString());
                Console.WriteLine(globalSpeed.ToString());

                Thread.Sleep(500);

                //ser.WriteLine(controlInt.ToString());
                //ser.WriteLine(turnSpeed.ToString());
                //set.WriteLine(globalSpeed.ToString());
            }
        }
    }
}
