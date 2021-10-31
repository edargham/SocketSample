using System;
using System.Threading.Tasks;
using Common;
using Common.XML;
using Common.JSON;
using Common.Enums;
using Server.Controllers;

namespace Server
{
    public class Handler
    {
        public static POSController PosController { get; set; }
        public static TransactionController TransController { get; set; }

        // Handler on the 'Server' side of the system.
        [XPathRoute("/Message[@type='Request' and @action='HeartBeat']")]
        [JPathRoute("$.action", "HeartBeat")]
        public static Task<HeartBeatResponseMessage<POSData>> HandleResponse(INetworkChannel channel, HeartBeatRequestMessage<POSData> request)
        {
            Received<HeartBeatRequestMessage<POSData>, POSData>(request);

            PosController.ProcessHeartBeat(request.Data.ID, channel);

            // Could be handled by a controller.
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

            Responded<HeartBeatResponseMessage<POSData>, POSData>(response);

            return Task.FromResult(response);
        }

        [XPathRoute("/Message[@type='Request' and @action='SubmitBasket']")]
        [JPathRoute("$.action", "SubmitBasket")]
        public static Task<SubmitBasketResponse> HandleMessage(SubmitBasketRequest request)
        {
            Received(request);

            TransController.AddSubmitBasketRequestToPool(request);

            var response = new SubmitBasketResponse
            {
                ID = request.ID,
                Data = request.Data,
                Result = new Result { Status = Status.Success }
            };

            Responded(response);
            
            return Task.FromResult(response);
        }

        private static void Received(Message<POSData> payload)
        {
            Console.WriteLine($"RECEIVED: {typeof(POSData).Name} => [Action: {payload.Action}, Request ID: {payload.ID}]");
        }

        private static void Responded(Message<POSData> payload)
        {
            Console.WriteLine($"SENT RESPONSE: {typeof(POSData).Name} => [Action: {payload.Action}, Request ID: {payload.ID}]");
        }

        private static void Received<T, TPayloadType>(T payload) where T : Message<TPayloadType> where TPayloadType : class
        {
            Console.WriteLine($"RECEIVED: {typeof(T).Name} => [Action: {payload.Action}, Request ID: {payload.ID}]");
        }

        private static void Responded<T, TPayloadType>(T payload) where T : Message<TPayloadType> where TPayloadType : class
        {
            Console.WriteLine($"SENT RESPONSE: {typeof(T).Name} => [Action: {payload.Action}, Request ID: {payload.ID}]");
        }
    }
}
