using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketServer server = new SocketServer();
            server.Start();

            Console.WriteLine("The Echo server is running....");
            Console.ReadLine();
        }
    }
}
