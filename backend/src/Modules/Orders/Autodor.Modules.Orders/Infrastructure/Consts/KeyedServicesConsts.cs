namespace Autodor.Modules.Orders.Infrastructure.Consts;

/// <summary>
/// Constants for keyed services registration.
/// </summary>
public static class KeyedServicesConsts
{
    /// <summary>
    /// Key for Products SOAP resilience pipeline.
    /// </summary>
    public const string ProductsSoap = nameof(ProductsSoap);

    /// <summary>
    /// Key for DistributorsSales SOAP resilience pipeline.
    /// </summary>
    public const string DistributorsSalesSoap = nameof(DistributorsSalesSoap);
}
