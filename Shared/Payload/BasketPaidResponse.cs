#nullable enable
using System.Xml.Serialization;
using Newtonsoft.Json;
using Common.Enums;

namespace Common
{
    [XmlRoot("Message")]
    public class BasketPaidResponse: Message<POSData>
    {
        [XmlElement("Result")]
        [JsonProperty("result")]
        public Result? Result { get; set; }

        public BasketPaidResponse()
        {
            Type = MessageType.Response;
            Action = "BasketPaid";
        }
    }
}
