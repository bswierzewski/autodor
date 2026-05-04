using Autodor.Modules.Orders.Infrastructure.ExternalServices.Products;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace Autodor.Tests.Integration.Modules.Orders.GenerateDeliveryNote;

public sealed class GenerateDeliveryNoteEnvironment : Shared.SharedEnvironment
{
    public Mock<IProductsClient> ProductsClientMock { get; } = new();

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        services.RemoveAll<IProductsClient>();
        services.AddSingleton(ProductsClientMock.Object);
    }
}