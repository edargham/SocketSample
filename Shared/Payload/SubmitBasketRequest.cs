# nullable enable
using System.Xml.Serialization;
using Newtonsoft.Json;
using Common.Enums;

namespace Common
{
    [XmlRoot("Message")]
    public class SubmitBasketRequest : Message<POSData>
    {
        [XmlAttribute("posTxnNumber")]
        [JsonProperty("posTxnNumber")]
        public string? POSTransactionNumber { get; set; }

        public SubmitBasketRequest()
        {
            Type = MessageType.Request;
            Action = "SubmitBasket";
        }
    }
}
