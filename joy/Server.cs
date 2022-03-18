using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DirectInput;


namespace joy
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;




    public class Server
    {
        private static IPHostEntry ipHostInfo;
        private static IPAddress ipAddress;
        private static IPEndPoint remoteEP;
        private static Socket sender;
        
        

        private static IPHostEntry IPAddresses(string server)
        {
            System.Text.ASCIIEncoding ASCII = new System.Text.ASCIIEncoding();
            // Get server related information.
            IPHostEntry heserver = Dns.GetHostEntry(server);
            return heserver;
        }

        private static void set(string server, int port)
        {
            // Establish the remote endpoint for the socket.  
            // This example uses port 11000 on the local computer.  
            ipHostInfo = IPAddresses(server);
            ipAddress = ipHostInfo.AddressList[0];
            remoteEP = new IPEndPoint(ipAddress, port);
            
        }

        public static void StartClient(string server, int port)
        {
            set(server,port);

            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            // Connect to a remote device.  
            try
            {
                // Create a TCP/IP  socket.  
                sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(remoteEP);
                Console.WriteLine("Socket connected to {0}",sender.RemoteEndPoint.ToString());
            }

            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                Console.WriteLine("Connection terminated. Press any key to continue");
                Console.ReadLine();
                Environment.Exit(0);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
                Console.WriteLine("Connection terminated. Press any key to continue");
                Console.ReadLine();
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
                Console.WriteLine("Connection terminated. Press any key to continue");
                Console.ReadLine();
                Environment.Exit(0);
            }
            bytes = new byte[1024];
     
        }

        public static void send(byte[] msg)
        {
            byte[] bytes = new byte[1024];
            // Send the data through the socket.  
            int bytesSent = sender.Send(msg);
            // Receive the response from the remote device.
            try
            {
                int bytesRec = sender.Receive(bytes);
                Console.WriteLine("Echoed test = {0}", Encoding.ASCII.GetString(bytes, 0, bytesRec));
            }
            catch
            {
            }

        }

        public static void StopClient()
        {
            // Release the socket.  
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

    }
}
