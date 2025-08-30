namespace Autodor.Modules.Products.Domain.ValueObjects;

public record Product(string Name, string PartNumber, string Ean)
{
    public static Product Empty => new(string.Empty, string.Empty, string.Empty);
};