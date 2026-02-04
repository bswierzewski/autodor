using Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.ServiceReference;

namespace Autodor.Modules.Orders.Infrastructure.Integrations.DistributorsSales.Factories;

/// <summary>
/// Factory interface for creating DistributorsSalesServiceClient instances.
///
/// Why use a factory pattern for WCF/SOAP clients?
///
/// 1. WCF clients are NOT thread-safe and should not be registered as Singleton
/// 2. WCF clients maintain connection state and must be properly closed/disposed
/// 3. ClientBase&lt;T&gt; implements IDisposable but throws exceptions in Dispose()
///    when in Faulted state, requiring special handling (Abort vs Close)
/// 4. Factory pattern provides:
///    - Full control over client lifecycle
///    - Consistent client creation logic
///    - Easier testing through interface abstraction
///    - Explicit creation instead of relying on DI scope management
/// </summary>
public interface IDistributorsSalesServiceClientFactory
{
    /// <summary>
    /// Creates a new instance of DistributorsSalesServiceClient with proper configuration.
    /// </summary>
    /// <returns>A new DistributorsSalesServiceClient instance.</returns>
    DistributorsSalesServiceClient Create();
}
