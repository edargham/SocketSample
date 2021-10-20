using Newtonsoft.Json;
using System.Xml.Serialization;

namespace Common
{
    public class PayloadMessage : Message
    {
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
