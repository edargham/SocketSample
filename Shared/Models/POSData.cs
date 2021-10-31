using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace Common
{
    public class POSData : Model
    {
        [XmlAttribute("ID")]
        [JsonProperty("id")]
        public string ID { get => $"PAYLOAD_ID_{_id}"; }

        [XmlAttribute("ItemName")]
        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [XmlAttribute("Price")]
        [JsonProperty("price")]
        public int Price { get; set; }

        [XmlAttribute("DatePurchased")]
        [JsonProperty("datePurchased")]
        public DateTime DatePurchased { get; set; }

        public override string ToString()
        {
            return $"ID: {ID}\nItem: {ItemName}\nPrice: {Price}\nDate Purchased: {DatePurchased.ToLongDateString()}";
        }
    }
}
