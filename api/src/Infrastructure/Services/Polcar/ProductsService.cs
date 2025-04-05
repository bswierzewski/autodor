using Application.Common.Interfaces;
using Application.Common.Options;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PolcarProductsClient;

namespace Infrastructure.Services.Polcar;

public class ProductsService(ILogger<ProductsService> logger, IOptions<PolcarOptions> polcarOptions) : IProductsService
{
    private readonly ProductsSoapClient _client = new(ProductsSoapClient.EndpointConfiguration.ProductsSoap12);

    public async Task<IDictionary<string, Domain.Entities.Product>> GetProductsAsync()
    {
        try
        {
            var response = await _client.GetEAN13ListAsync(
                        polcarOptions.Value.Login,
                        polcarOptions.Value.Password,
                        1,
                        1);

            var products = response.Body.GetEAN13ListResult.Deserialize<ProductRoot>();

            return products.Items.Select(x => new Domain.Entities.Product
            {
                EAN13Code = x.EAN13Code,
                Name = x.PartName,
                Number = x.Number
            }).ToDictionary(x => x.Number);
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex.Message);

            return new Dictionary<string, Domain.Entities.Product>();
        }
    }
}
