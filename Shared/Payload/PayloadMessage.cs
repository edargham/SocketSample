using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace Common
{
    public class PayloadMessage : Payload
    {
        [XmlAttribute("ID")]
        [JsonProperty("id")]
        public string ID { get => $"PAYLOAD_ID_{_id}"; }

        [XmlAttribute("intprop")]
        [JsonProperty("intProp")]
        public int IntProp { get; set; }

        [XmlAttribute("strprop")]
        [JsonProperty("strProp")]
        public string StrProp { get; set; }

        public override string ToString()
        {
            return $"{IntProp} - {StrProp}";
        }
    }
}
