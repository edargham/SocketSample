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

        private static async Task RequestPayload(int interval)
        {
            Guid requestID = Guid.NewGuid();
            
            while(true)
            {
                PayloadMessage payload = new PayloadMessage
                {
                    IntProp = 69420,
                    StrProp = "Nice",
                };

                HeartBeatRequestMessage<PayloadMessage> heartbeatRequest = new HeartBeatRequestMessage<PayloadMessage>
                {
                    ID = requestID.ToString(),
                    Data = payload
                };

                Console.WriteLine("Sending the payload to the echo server...");

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
            Console.ReadLine();

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
                    await RequestPayload(5);
                }
            );

            Console.WriteLine("Press any key to continue...");
            Console.ReadLine();
        }
    }
}
