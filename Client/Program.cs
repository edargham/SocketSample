using Common;
using Common.JSON;
using Common.JSONChannels;
using Common.XML;
using Common.XMLChannels;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        //private static readonly XMLClientChannel _xmlClientChannel = new XMLClientChannel();
        //private static readonly XMLDispatcher _xmlDispatcher = new XMLDispatcher();
        private static readonly JSONClientChannel _jsonClientChannel = new JSONClientChannel();
        private static readonly JSONDispatcher _jsonDispatcher = new JSONDispatcher();

        private static int _pid = System.Diagnostics.Process.GetCurrentProcess().Id;

        private static async Task RequestPayload(int interval, int numPayloadsToSend)
        {
            Guid requestID = Guid.NewGuid();

            Random rng = new Random();

            bool stopCondition() => numPayloadsToSend < 0 || numPayloadsToSend-- > 0;

            while(stopCondition())
            {
                POSData payload = new POSData
                {
                    _id = _pid,
                    ItemName = "Foo",
                    Price = rng.Next(0, 1001),
                    DatePurchased = DateTime.Now
                };

                HeartBeatRequestMessage<POSData> heartbeatRequest = new HeartBeatRequestMessage<POSData>
                {
                    ID = requestID.ToString(),
                    Data = payload
                };

                Console.WriteLine("Sending the following payload to the echo server...");
                Console.WriteLine($"Data to send:\n=================\n{ payload }\n-----------------\n");
                //await _xmlClientChannel.SendAsync(heartbeatRequest).ConfigureAwait(false);
                await _jsonClientChannel.SendAsync(heartbeatRequest).ConfigureAwait(false);

                await Task.Delay(interval * 1000);
            }
        }

        public static async Task Main(string[] args)
        {
            //_xmlDispatcher.Register<HeartBeatResponseMessage<PayloadMessage>>(Handler.HeartBeatResponseHandler);
            //_jsonDispatcher.Register<HeartBeatResponseMessage<PayloadMessage>>(Handler.HeartBeatResponseHandler);
            
            //_xmlDispatcher.Bind<Handler>();
            _jsonDispatcher.Bind<Handler>();

            Console.WriteLine("Press any key to contact the echo server...");
            Console.ReadKey();

            try
            {


                IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 42369);

                //_xmlDispatcher.Bind(_xmlClientChannel);
                _jsonDispatcher.Bind(_jsonClientChannel);

                //_xmlClientChannel.SetCallBack(_xmlDispatcher.DispatchAsync);
                //_jsonClientChannel.SetCallBack(_jsonDispatcher.DispatchAsync);

                //await _xmlClientChannel.ConnectAsync(endPoint).ConfigureAwait(false);
                await _jsonClientChannel.ConnectAsync(endPoint).ConfigureAwait(false);

                _ = Task.Run(
                    async () =>
                    {
                        await RequestPayload(5, -1);
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to establish a connection to the server.\nReason:\n{ex}");
            }

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
