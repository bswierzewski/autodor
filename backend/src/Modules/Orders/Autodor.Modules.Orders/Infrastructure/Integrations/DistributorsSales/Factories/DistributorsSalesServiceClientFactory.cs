using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.ServiceReference;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Factories;

/// <summary>
/// Factory for creating DistributorsSalesServiceClient instances.
/// </summary>
public class DistributorsSalesServiceClientFactory : IDistributorsSalesServiceClientFactory
{
    /// <inheritdoc />
    public DistributorsSalesServiceClient Create() 
        => new(DistributorsSalesServiceClient.EndpointConfiguration.BasicHttpBinding_IDistributorsSalesService_soap);
}
