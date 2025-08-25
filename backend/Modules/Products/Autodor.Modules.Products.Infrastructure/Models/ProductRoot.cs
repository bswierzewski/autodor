using System.Xml.Serialization;

namespace Autodor.Modules.Products.Infrastructure.Models;

[XmlRoot("ROOT")]
public class ProductRoot
{
    [XmlElement("ITEM")]
    public Item[] Items { get; set; } = null!;
}

public class Item
{
    [XmlAttribute]
    public string Number { get; set; } = null!;

    [XmlAttribute]
    public string PartName { get; set; } = null!;

    [XmlAttribute]
    public string EAN13Code { get; set; } = null!;
}