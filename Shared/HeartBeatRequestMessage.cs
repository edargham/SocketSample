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
    public class HeartBeatRequestMessage<T> : Message where T : class
    {
        /// <summary>
        /// The object to be passed as a heartbeat message.
        /// </summary>
        [XmlElement("Data")]
        [JsonProperty("data")]
        public T? Data { get; set; }

        public HeartBeatRequestMessage()
        {
            Type = MessageType.Request;
            Action = "HeartBeat";
        }
    }
}
