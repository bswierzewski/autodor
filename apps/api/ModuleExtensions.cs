using Autodor.Modules.Contractors;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Modules.Errors;
using Autodor.Modules.Invoicing;
using Autodor.Modules.Orders;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Infrastructure.Middleware;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Persistence.Extensions;

namespace Autodor.API;

public static class ModuleExtensions
{
    public static IServiceCollection ConfigureModules(
        this IServiceCollection services,
        IConfiguration configuration,
        out IModule[] modules)
    {
        // Modules are composed explicitly so bootstrapping stays predictable.
        modules =
        [
            new ContractorsModule(),
            new ErrorsModule(),
            new OrdersModule(),
            new InvoicingModule()
        ];

        foreach (var module in modules)
        {
            services.AddSingleton(typeof(IModule), module);

            if (module is IEndpointModule endpointModule)
                services.AddSingleton(typeof(IEndpointModule), endpointModule);

            module.AddServices(services, configuration);
        }

        return services;
    }

    public static async Task InitializeModulesAsync(
        this WebApplication app,
        IModule[] modules)
    {
        foreach (var module in modules)
            await module.InitializeAsync(app.Services);
    }

    public static async Task ApplyMigrations(this WebApplication app)
    {
        // Migrations stay explicit here so startup only touches owned module schemas.
        Func<IServiceProvider, CancellationToken, Task>[] migrations =
        [
            static (services, cancellationToken) => services.MigrateDatabaseAsync<ContractorsDbContext>(cancellationToken),
            static (services, cancellationToken) => services.MigrateDatabaseAsync<OrdersDbContext>(cancellationToken)
        ];

        foreach (var migration in migrations)
            await migration(app.Services, CancellationToken.None);
    }
}