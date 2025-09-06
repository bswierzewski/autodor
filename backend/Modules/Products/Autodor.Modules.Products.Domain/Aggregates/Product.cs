using BuildingBlocks.Domain.Primitives;

namespace Autodor.Modules.Products.Domain.Aggregates;

public class Product : AggregateRoot<int>
{
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EAN13 { get; set; } = string.Empty;
}