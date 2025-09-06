using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Products.Domain.Aggregates;

public class Product : AggregateRoot<int>
{
    public string PartNumber { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Ean { get; set; } = string.Empty;
}