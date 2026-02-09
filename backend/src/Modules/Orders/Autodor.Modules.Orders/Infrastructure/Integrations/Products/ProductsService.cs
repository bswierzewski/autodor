using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Dtos;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Models;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Options;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.ServiceReference;
using BuildingBlocks.Infrastructure.Soap;
using BuildingBlocks.Infrastructure.Soap.Abstractions;
using BuildingBlocks.Kernel.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public class ProductsService(
    IOptions<ProductsOptions> options,
    ISoapInvoker<ProductsSoapClient> soapInvoker,
    ILogger<ProductsService> logger
    ) : IProductsService
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
