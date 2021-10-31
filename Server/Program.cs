using System;
using System.Threading;
using System.Threading.Tasks;
using Common.JSON.Server;
using Server.Controllers;

namespace Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            POSController posController = new POSController();

            TransactionController transactionController = new TransactionController
            {
                PosController = posController
            };

            Handler.PosController = posController;
            Handler.TransController = transactionController;

            CancellationTokenSource cancellationToken= new CancellationTokenSource();
            JSONSocketServer server = new JSONSocketServer(3);
            server.Bind<Handler>();

            Task serverTask = server.StartAsync(42369, cancellationToken.Token);

            Console.WriteLine("The Echo server is running...\nPress ENTER to exit.\nPress F to accept a payment.");
            do
            {
                ConsoleKeyInfo key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        cancellationToken.Cancel();
                        await serverTask;
                        Console.WriteLine("Exiting...");
                        return;

                    case ConsoleKey.F:
                        await transactionController.PayBasket().ConfigureAwait(false);
                        break;
                }
            } while (true);
        }
    }
}
