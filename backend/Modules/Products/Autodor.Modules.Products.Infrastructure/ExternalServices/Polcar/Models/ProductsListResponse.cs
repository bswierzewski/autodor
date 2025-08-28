using System.Xml.Serialization;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Models;

[XmlRoot("ProductsList")]
public class ProductRoot
{
    [XmlElement("Item")]
    public List<ProductItem> Items { get; set; } = new();
}

public class ProductItem
{
    [XmlElement("Number")]
    public string Number { get; set; } = string.Empty;

    [XmlElement("PartName")]
    public string PartName { get; set; } = string.Empty;

    [XmlElement("EAN13Code")]
    public string EAN13Code { get; set; } = string.Empty;

    [XmlElement("Brand")]
    public string Brand { get; set; } = string.Empty;

    [XmlElement("Price")]
    public decimal Price { get; set; }

    [XmlElement("Currency")]
    public string Currency { get; set; } = string.Empty;
}