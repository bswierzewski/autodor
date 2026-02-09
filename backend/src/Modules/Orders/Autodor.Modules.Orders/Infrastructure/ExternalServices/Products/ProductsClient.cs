using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Dtos;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Models;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.ServiceReference;
using BuildingBlocks.Infrastructure.Soap.Abstractions;
using BuildingBlocks.Kernel.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public class ProductsClient(
    IOptions<ProductsOptions> options,
    ISoapInvoker<ProductsSoapClient> soapInvoker,
    ILogger<ProductsClient> logger
    ) : IProductsClient
{
    private readonly ProductsOptions _options = options.Value;

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        try
        {
            var response = await soapInvoker.InvokeAsync(async client =>
            {
                return await client.GetEAN13ListAsync(
                    Login: _options.Login,
                    Password: _options.Password,
                    LanguageID: _options.LanguageId,
                    FormatID: _options.FormatId);
            });

            var deserialized = response.Body.GetEAN13ListResult.OuterXml.FromXml<ProductRoot>();

            return deserialized.Items?
                .Select(item => new ProductDto
                {
                    Number = item.Number,
                    Name = item.PartName,
                    EAN13 = item.EAN13Code
                })
                .ToList() ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while loading products from Polcar");
            return [];
        }
    }
}
