using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DirectInput;




namespace joy
{
    public class fullAuto
    {
        public static bool shoot = false;
    }
    public class InputLoop
    {
        
        public static async Task JoyIn()
        {
            // Initialize DirectInput
            var directInput = new DirectInput();

            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,
                        DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;

            // If Gamepad not found, look for a Joystick
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,
                        DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                Console.WriteLine("No joystick/Gamepad found.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            // Instantiate the joystick
            var joystick = new Joystick(directInput, joystickGuid);

            Console.WriteLine("Found Joystick/Gamepad with GUID: {0}", joystickGuid);

            // Query all suported ForceFeedback effects
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                Console.WriteLine("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();

            
            async Task FullAuto()
            {
                if(fullAuto.shoot == true)
                {
                    string r = "128";
                    byte[] msg = Encoding.ASCII.GetBytes(r);
                    Console.WriteLine("Shoot!");
                    Server.send(msg);
                }
            }

            async Task MovOut(JoystickUpdate State)
            {

                int o = State.Value;
                string r = Convert.ToString(State);
                byte[] msg = Encoding.ASCII.GetBytes(r);
                FullAuto();
                if (State.Offset == JoystickOffset.X)
                {

                    if (o >= 33000)
                    {
                        Console.WriteLine("x greater");
                        Server.send(msg);

                    }
                    else if (o <= 31000)
                    {
                        Console.WriteLine("x less");
                        Server.send(msg);
                    }
                    else
                    {
                        Console.WriteLine("x deadzone");
                        Server.send(msg);
                    }
                }
                if (State.Offset == JoystickOffset.Y)
                {
                    Console.WriteLine(State.Value);
                    if (o >= 33000)
                    {
                        Console.WriteLine("y greater");
                        Server.send(msg);

                    }
                    else if (o <= 31000)
                    {
                        Console.WriteLine("y less");
                        Server.send(msg);
                    }
                    else
                    {
                        Console.WriteLine("y deadzone");
                        Server.send(msg);
                    }
                }
                if (State.Offset == JoystickOffset.Buttons0)
                {
                    if (State.Value == 128)
                    {
                        Console.WriteLine(State.Value);
                        fullAuto.shoot = true;
                        //Server.send(msg);
                    }
                    else
                    {
                        Console.WriteLine(State.Value);
                        Console.WriteLine("Cease Fire!");
                        fullAuto.shoot = false;
                    }
                }

                if (State.Offset == JoystickOffset.Buttons1)
                {
                    msg = Encoding.ASCII.GetBytes("q");
                    Server.send(msg);
                    Thread.Sleep(1000);
                    Server.StopClient();
                    
                    Console.WriteLine("Connection terminated. Press any key to continue");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                if (State.Offset == JoystickOffset.Buttons2)
                {
                    msg = Encoding.ASCII.GetBytes("r");
                    Server.send(msg);
                    Thread.Sleep(1000);
                    Server.StopClient();
                    
                    Console.WriteLine("Connection Restarted. Press any key to continue");
                    Console.WriteLine("This feature is not supported yet");
                    Console.WriteLine("Please restart the Pi server and all other apps manually");
                    Console.ReadLine();
                    return;
                }
            }

            // Poll events from joystick
            async Task JoyOut(JoystickUpdate[] Datas)
            {
                foreach (var state in Datas)
                {
                    MovOut(state);
                }

            }

            // Poll events from joystick
            while (true)
            {
                joystick.Poll();
                var datas = joystick.GetBufferedData();
                JoyOut(datas);

            }

        }
    }
}
