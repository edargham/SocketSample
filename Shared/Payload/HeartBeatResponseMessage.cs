#nullable enable
using Common.Enums;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Common
{
    /// <summary>
    /// Heartbeat messages are messages that are sent through a socket to keep a connection alive.
    /// </summary>
    [XmlRoot("Message")]
    public class HeartBeatResponseMessage<T> : Message<T> where T : class
    {
        [XmlElement("Result")]
        [JsonProperty("result")]
        public Result? Result { get; set; }

        public HeartBeatResponseMessage()
        {
            Type = MessageType.Response;
            Action = "HeartBeat";
        }
    }
}
