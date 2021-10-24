using System;
using Common.JSON.Server;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            JSONSocketServer server = new JSONSocketServer();
            server.Bind<Handler>();
            server.Start();

            Console.WriteLine("The Echo server is running....");
            Console.ReadLine();
        }
    }
}
