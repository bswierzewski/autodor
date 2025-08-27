using Autodor.Modules.Products.Application.Queries.GetProducts;
using MediatR;

namespace Autodor.Web.Endpoints;

public static class ProductsEndpoints
{
    public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", async (string[] partNumbers, IMediator mediator) =>
        {
            var products = await mediator.Send(new GetProductsQuery(partNumbers));
            return Results.Ok(products);
        })
        .WithName("GetProducts")
        .WithSummary("Get products by part numbers");

        group.MapGet("/search", async (IMediator mediator, string? searchTerm = null, int page = 1, int pageSize = 20) =>
        {
            // Dla uproszczenia używamy tego samego query - w rzeczywistej implementacji byłby osobny SearchProductsQuery
            var allProducts = await mediator.Send(new GetProductsQuery(Array.Empty<string>()));
            
            var filteredProducts = allProducts.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredProducts = filteredProducts.Where(p => 
                    p.PartNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            var pagedResults = filteredProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return Results.Ok(new 
            { 
                Products = pagedResults,
                Page = page,
                PageSize = pageSize,
                Total = filteredProducts.Count()
            });
        })
        .WithName("SearchProducts")
        .WithSummary("Search products with pagination");

        return app;
    }
}