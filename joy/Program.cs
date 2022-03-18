using SharpDX;
using SharpDX.DirectInput;
using joy;


class Joy
{
 
    static void firstThingsFirst() //This Function is basically just to allow us to enter debugging mode so we can test the local app without connecting to ccait
    {                              //This can be done by typing "debug" when prompted for an IP address
        string debug = "debug";
        Console.WriteLine("Welcome to CCAIT Comand and Control Interface");
        Console.WriteLine("");
        Console.Write("CCAIT IP: ");
        string ip = Console.ReadLine();
        if(ip.Contains(debug)==true)
        {
            Console.WriteLine("debugger not fully supported");
        }
        else
        {
            config(ip);
        }
    }
    static void config(string ip)
    {
        
        try
        {
            Console.WriteLine("Conecting to CCAIT...");
            Server.StartClient(ip, 9999);
            joy.InputLoop.JoyIn();
        }
        catch
        {
            Console.WriteLine("Unable to Connect");
            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
        }
    }
    static void Main()
    {
        do
        {
            try
            {
                firstThingsFirst();
                joy.InputLoop.JoyIn();
            }
            catch
            {
                Console.WriteLine("Connection terminated");
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();
                Environment.Exit(0);
            }
        }
        while (true);
    }
}
