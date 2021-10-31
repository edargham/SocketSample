#nullable enable
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Common
{
    public class PaymentInfo
    {
        [XmlAttribute("AuthCode")]
        [JsonProperty("authCode")]
        public string? AuthorizationCode { get; set; }

        [XmlAttribute("Amount")]
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [XmlAttribute("LastFourDigits")]
        [JsonProperty("lastFourDigits")]
        public string? LastFourDigits { get; set; }
    }
}