using System;
using System.Net;
using System.Net.Sockets;

namespace CyfroweBanknoty.Tools
{
    public class Connection
    {
        public Socket sock;
        public Socket handler;
        public IPEndPoint ipEndPoint;

        public Connection()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("");

            Console.WriteLine("------------------");
            Console.WriteLine("All available IPs:");
            foreach (IPAddress addr in ipHostInfo.AddressList)
                Console.WriteLine("ip: " + addr);
            Console.WriteLine("------------------\n");

            IPAddress ipAddress = ipHostInfo.AddressList[1];
            ipEndPoint = new IPEndPoint(ipAddress, 4445);
            Console.WriteLine(ipEndPoint);

            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        ~Connection()
        {
            Console.WriteLine("[info] Shutting down the connection...");
            sock.Close();
        }

        public byte[] Receive(int socket)
        {
            byte[] bytes = new byte[1024];
            int bytes_received;

            switch (socket)
            {
                case 0:
                    bytes_received = handler.Receive(bytes);
                    break;
                case 1:
                    bytes_received = sock.Receive(bytes);
                    break;
                default:
                    bytes_received = 0;
                    break;
            }

            Array.Resize(ref bytes, bytes_received);
            return bytes;
        }

        public void Send(int socket, byte[] bytes)
        {
            switch (socket)
            {
                case 0:
                    handler.Send(bytes);
                    break;
                case 1:
                    sock.Send(bytes);
                    break;
            }
        }
    }
}