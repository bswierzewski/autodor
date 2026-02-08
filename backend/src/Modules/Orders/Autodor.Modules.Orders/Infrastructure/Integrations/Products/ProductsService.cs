using Autodor.Modules.Orders.Infrastructure.Consts;
using Autodor.Modules.Orders.Infrastructure.Extensions;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Dtos;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Factories;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Models;
using Autodor.Modules.Orders.Infrastructure.Integrations.Products.Options;
using BuildingBlocks.Kernel.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public class ProductsService(
    IOptions<ProductsOptions> options,
    IProductsSoapClientFactory clientFactory,
    [FromKeyedServices(KeyedServicesConsts.ProductsSoap)] ResiliencePipeline resiliencePipeline,
    ILogger<ProductsService> logger
    ) : IProductsService
{
    private readonly ProductsOptions _options = options.Value;

    public async Task<IEnumerable<ProductDto>> GetProductsAsync()
    {
        var client = clientFactory.Create();

        try
        {
            var response = await resiliencePipeline.ExecuteAsync(async ct =>
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
        finally
        {
            await client.CloseClientAsync();
        }
    }
}
