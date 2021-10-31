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
    public class HeartBeatRequestMessage<T> : Message<T> where T : class
    {
        public HeartBeatRequestMessage()
        {
            Type = MessageType.Request;
            Action = "HeartBeat";
        }
    }
}
