using System.Xml.Serialization;

namespace Infrastructure.Models
{
    [XmlRoot("ROOT")]
    public class ProductRoot
    {
        [XmlElement("ITEM")]
        public Item[] Items { get; set; }
    }

    public class Item
    {
        [XmlAttribute]
        public string Number { get; set; }

        [XmlAttribute]
        public string PartName { get; set; }

        [XmlAttribute]
        public string EAN13Code { get; set; }
    }
}
