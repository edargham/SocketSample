#nullable enable
using Common.Enums;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Common
{
    /// <summary>
    /// The format all messages passed to the routes must follow.
    /// </summary>
    [XmlRoot("Message")]
    public abstract class Message
    {
        [XmlAttribute("id")]
        [JsonProperty("id")]
        public string? ID { get; set; }

        [XmlAttribute("type")]
        [JsonProperty("type")]
        public MessageType Type { get; set; }

        [XmlAttribute("action")]
        [JsonProperty("action")]
        public string? Action { get; set; }
    }
}
