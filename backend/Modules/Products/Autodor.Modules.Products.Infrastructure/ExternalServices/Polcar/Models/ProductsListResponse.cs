using System.Xml.Serialization;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Models;

[XmlRoot("ROOT")]
public class ProductRoot
{
    [XmlElement("ITEM")]
    public List<ProductItem> Items { get; set; } = new();
}

public class ProductItem
{
    [XmlAttribute("Number")]
    public string Number { get; set; } = string.Empty;

    [XmlAttribute("PartName")]
    public string PartName { get; set; } = string.Empty;

    [XmlAttribute("EAN13Code")]
    public string EAN13Code { get; set; } = string.Empty;
}