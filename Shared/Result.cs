using Common.Enums;
using System.Xml.Serialization;

namespace Common
{
    public class Result
    {
        [XmlAttribute("status")]
        public Status Status { get; set; }
    }
}
