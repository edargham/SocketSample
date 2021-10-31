#nullable enable
using System.Xml.Serialization;
using Newtonsoft.Json;
using Common.Enums;

namespace Common
{
    [XmlRoot("Message")]
    public class BasketPaidRequest : Message<POSData>
    {
        [XmlAttribute("posTxnNumber")]
        [JsonProperty("posTxnNumber")]
        public string? POSTransactionNumber { get; set; }

        [XmlAttribute("paymentInformation")]
        [JsonProperty("paymentInformation")]
        public PaymentInfo? PaymentInformation { get; set; }

        public BasketPaidRequest()
        {
            Type = MessageType.Request;
            Action = "BasketPaid";
        }
    }
}
