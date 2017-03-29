using System;
using System.IO.Ports;
using System.Security.AccessControl;
using SharpDX.XInput;

namespace RobotController
{
    class Program
    {
        static void Main(string[] args)
        {
            bool[] controls = new bool[8];

            SerialPort ser = new SerialPort();
            ser.BaudRate = 9600;
            ser.PortName = "COM7";
            ser.Open();

            Console.WriteLine("Start XGamepadApp");
            // Initialize XInput
            var controllers = new[] { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };

            // Get 1st controller available
            Controller controller = null;
            foreach (var selectControler in controllers)
            {
                if (selectControler.IsConnected)
                {
                    controller = selectControler;
                    break;
                }
            }

            if (controller == null)
            {
                Console.WriteLine("No XInput controller installed");
            }
            else
            {

                Console.WriteLine("Found a XInput controller available");
                Console.WriteLine("Press buttons on the controller to display events or escape key to exit... ");

                // Poll events from joystick
                while (controller.IsConnected)
                {
                    var state = controller.GetState();
                    Gamepad gamepad = state.Gamepad;

                    

                    int turnSpeed = gamepad.LeftThumbX / 128;
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


                    int globalSpeed = Math.Abs(gamepad.LeftTrigger - gamepad.RightTrigger);

                    if (gamepad.LeftTrigger > gamepad.RightTrigger)
                    {
                        headingForward = false;
                    }

                    if (turnSpeed > 60 || globalSpeed > 0)
                    {
                        move = true;
                    }

                    controls[0] = move;
                    controls[1] = headingForward;
                    controls[2] = left;
                    controls[3] = right;
                    controls[4] = controls[5] = controls[6] = controls[7] = false;;

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

                    ser.WriteLine(controlInt.ToString());
                    ser.WriteLine(turnSpeed.ToString());
                }
            }
            Console.WriteLine("End XGamepadApp");
            ser.Close();
        }
    }
}
