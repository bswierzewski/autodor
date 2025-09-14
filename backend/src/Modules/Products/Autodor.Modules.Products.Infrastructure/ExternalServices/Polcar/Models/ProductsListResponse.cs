using System.Xml.Serialization;

namespace Autodor.Modules.Products.Infrastructure.ExternalServices.Polcar.Models;

/// <summary>
/// Represents the root element for Polcar product list XML response.
/// </summary>
[XmlRoot("ROOT")]
public class ProductRoot
{
    /// <summary>
    /// Gets or sets the collection of product items from the Polcar response.
    /// </summary>
    [XmlElement("ITEM")]
    public List<ProductItem> Items { get; set; } = new();
}

/// <summary>
/// Represents a single product item in the Polcar XML response with essential product attributes.
/// </summary>
public class ProductItem
{
    /// <summary>
    /// Gets or sets the unique product number identifier.
    /// </summary>
    [XmlAttribute("Number")]
    public string Number { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name of the product part.
    /// </summary>
    [XmlAttribute("PartName")]
    public string PartName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the EAN13 barcode for the product.
    /// </summary>
    [XmlAttribute("EAN13Code")]
    public string EAN13Code { get; set; } = string.Empty;
}