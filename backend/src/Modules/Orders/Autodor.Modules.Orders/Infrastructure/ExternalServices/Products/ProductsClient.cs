using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.Options;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.ServiceReference;
using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products.ServiceReference.Models;
using BuildingBlocks.Soap;
using BuildingBlocks.Soap.Abstractions;
using BuildingBlocks.Kernel.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Frozen;

namespace Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;

/// <summary>
/// Service for products external integration
/// </summary>
public class ProductsClient(
    IOptions<ProductsOptions> options,
    SoapPipeline<ProductsSoapClient> soapPipeline,
    ILogger<ProductsClient> logger
    ) : IProductsClient
{
    private readonly ProductsOptions _options = options.Value;

    public async Task<FrozenDictionary<string, Models.Product>> GetProductsAsync()
    {
        try
        {
            var response = await soapPipeline.InvokeAsync(
                async client =>
                {
                    return await client.GetEAN13ListAsync(
                        Login: _options.Login,
                        Password: _options.Password,
                        LanguageID: _options.LanguageId,
                        FormatID: _options.FormatId);
                },
                SoapCallContext.ForCache(
                    "GetEAN13List",
                    _options.Login,
                    _options.Password,
                    _options.LanguageId,
                    _options.FormatId));

            return response.Body.GetEAN13ListResult.OuterXml.FromXml<ProductRoot>()
                .Items
                .Select(xml => new Models.Product
                {
                    Number = xml.Number,
                    Name = xml.PartName,
                    EAN13 = xml.EAN13Code
                })
                .GroupBy(p => p.Number)
                .ToFrozenDictionary(g => g.Key, g => g.Last());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while loading products from Polcar");
            return FrozenDictionary<string, Models.Product>.Empty;
        }
    }
}
