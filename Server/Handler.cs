using System;
using System.Threading.Tasks;
using Common;
using Common.XML;
using Common.JSON;
using Common.Enums;

namespace Server
{
    public static class Handler
    {
        // Handler on the 'Server' side of the system.
        [XPathRoute("/Message[@type='Request' and @action='HeartBeat']")]
        [JPathRoute("$.action", "HeartBeat")]
        public static Task<HeartBeatResponseMessage<PayloadMessage>> HandleResponse(HeartBeatRequestMessage<PayloadMessage> request)
        {
            Received(request);

            HeartBeatResponseMessage<PayloadMessage> response = new HeartBeatResponseMessage<PayloadMessage>
            {
                ID = request.ID,
                Result = new Result { Status = Status.Success }
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
