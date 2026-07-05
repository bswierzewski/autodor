using Autodor.Modules.Contractors;
using Autodor.Modules.Errors;
using Autodor.Modules.Invoicing;
using Autodor.Modules.Orders;
using BuildingBlocks.Core.Interfaces;

namespace Autodor.Bootstrap;

/// <summary>
/// Defines the explicit set of application modules that make up the Autodor backend.
/// </summary>
public static class ModuleCatalog
{
    /// <summary>
    /// Creates the module instances used by runtime hosts and tooling.
    /// </summary>
    public static IModule[] CreateModules() =>
    [
        new ContractorsModule(),
        new ErrorsModule(),
        new OrdersModule(),
        new InvoicingModule()
    ];
}
