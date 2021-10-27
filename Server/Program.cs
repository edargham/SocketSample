using System;
using System.Threading;
using System.Threading.Tasks;
using Common.JSON.Server;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CancellationTokenSource cancellationToken= new CancellationTokenSource();
            JSONSocketServer server = new JSONSocketServer(3);
            server.Bind<Handler>();

            Task serverTask = server.StartAsync(42369, cancellationToken.Token);

            do
            {
                Console.WriteLine("The Echo server is running... Press enter to exit.");
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    cancellationToken.Cancel();
                    await serverTask;
                    Console.WriteLine("Exiting...");
                    break;
                }
            } while (true);
        }
    }
}
