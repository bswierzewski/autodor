namespace Autodor.Modules.Products.Domain.ValueObjects;

public record Product(string Name, string PartNumber)
{
    public static Product Empty => new(string.Empty, string.Empty);
};