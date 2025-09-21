using Autodor.Shared.Contracts.Products.Dtos;

namespace Autodor.Modules.Products.Application.API;

public static class MappingExtensions
{
    public static ProductDetailsDto ToDto(this Domain.Aggregates.Product product)
    {
        return new ProductDetailsDto(
            Number: product.Number,
            Name: product.Name,
            EAN13: product.EAN13
        );
    }
}