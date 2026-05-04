using Autodor.Modules.Contractors;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Modules.Invoicing;
using Autodor.Modules.Orders;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Infrastructure.Middleware;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using BuildingBlocks.Infrastructure.Wolverine.Extensions;

namespace Autodor.API;

public static class ModuleExtensions
{
    // OpenAPI startup path does not need a dedicated Postgres data source for Wolverine.
    public static void ConfigureOpenApiWolverine(
        this WebApplicationBuilder builder,
        IModule[] modules)
    {
        builder.AddWolverine(modules, opts => opts.Discovery.IncludeAssembly(typeof(AuthorizeAttribute).Assembly));
    }

    // Runtime startup uses a shared Postgres-backed Wolverine data source.
    public static void ConfigureWolverine(
        this WebApplicationBuilder builder,
        IModule[] modules)
    {
        var dataSource = builder.Services.AddPostgresDataSource(builder.Configuration, "Default");
        builder.AddWolverine(modules, dataSource, opts => opts.Discovery.IncludeAssembly(typeof(AuthorizeAttribute).Assembly));
    }

    public static IServiceCollection ConfigureModules(
        this IServiceCollection services,
        IConfiguration configuration,
        out IModule[] modules)
    {
        // Modules are composed explicitly so bootstrapping stays predictable.
        modules =
        [
            new ContractorsModule(),
            new OrdersModule(),
            new InvoicingModule()
        ];

        foreach (var module in modules)
        {
            services.AddSingleton(typeof(IModule), module);
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