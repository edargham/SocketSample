#nullable enable
using System.Xml.Serialization;
using Newtonsoft.Json;
using Common.Enums;

namespace Common
{
    [XmlRoot("Message")]
    public class SubmitBasketResponse : Message<POSData>
    {
        [XmlElement("Result")]
        [JsonProperty("result")]
        public Result? Result { get; set; }

        public SubmitBasketResponse()
        {
            Type = MessageType.Response;
            Action = "SubmitBasket";
        }
    }
}
