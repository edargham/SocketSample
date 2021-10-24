using System;
using System.Threading.Tasks;
using Common;
using Common.XML;
using Common.JSON;
using Common.Enums;

namespace Server
{
    public class Handler
    {
        // Handler on the 'Server' side of the system.
        [XPathRoute("/Message[@type='Request' and @action='HeartBeat']")]
        [JPathRoute("$.action", "HeartBeat")]
        public static Task<HeartBeatResponseMessage<POSData>> HandleResponse(HeartBeatRequestMessage<POSData> request)
        {
            Received(request);
            if(request.Data != null)
            {
                Console.WriteLine($"CLIENT ID: {request.Data.ID}");
                request.Data.Price += (int)(request.Data.Price * 0.1); 
            }

            HeartBeatResponseMessage<POSData> response = new HeartBeatResponseMessage<POSData>
            {
                ID = request.ID,
                Result = new Result { Status = Status.Success },
                Data = request.Data
            };

            Responded(response);

            return Task.FromResult(response);
        }

        private static void Received<T>(T payload) where T : Message
        {
            Console.WriteLine($"RECEIVED: {typeof(T).Name} => [Action: {payload.Action}, Request ID: {payload.ID}]");
        }

        private static void Responded<T>(T payload) where T : Message
        {
            Console.WriteLine($"SENT RESPONSE: {typeof(T).Name} => [Action: {payload.Action}, Request ID: {payload.ID}]");
        }
    }
}
