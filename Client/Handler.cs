using Common;
using Common.XML;
using Common.JSON;
using System;
using System.Threading.Tasks;

namespace Client
{
    public class Handler
    {
        // Handler on the 'Client' side of the system.
        [XPathRoute("/Message[@type='Response' and @action='HeartBeat']")]
        [JPathRoute("$.action", "HeartBeat")]
        public static Task HeartBeatResponseHandler(HeartBeatResponseMessage<POSData> response)
        {
            Console.WriteLine($"Received Response: {response?.Result?.Status}, {response?.ID}");
            Console.WriteLine($"Recieved Data:\n=================\n{response?.Data.ToString()}\n-----------------\n");
            return Task.CompletedTask;
        }
    }
}
