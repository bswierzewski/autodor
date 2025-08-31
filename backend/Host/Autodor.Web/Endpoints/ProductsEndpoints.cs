using Autodor.Modules.Products.Application.Queries.GetProduct;
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


        group.MapGet("/{partNumber}", async (string partNumber, IMediator mediator) =>
        {
            var product = await mediator.Send(new GetProductQuery(partNumber));
            // Rozważenie obsługi przypadku, gdy produkt nie zostanie znaleziony
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProduct") // Zmieniono nazwę na unikalną
        .WithSummary("Get product by part number"); // Zaktualizowano podsumowanie

        return app;
    }
}