using Autodor.Modules.Invoicing.Domain;
using Autodor.Modules.Invoicing.Infrastructure.Database;
using Autodor.Modules.Invoicing.Infrastructure.Options;
using Autodor.Shared.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing;

public static class InvoicingModuleExtensions
{
    public static IServiceCollection AddInvoicingModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddModule(configuration, Module.Name)
            .AddOptions(opts => { })
            .AddPostgres<InvoicingDbContext, DatabaseOptions>();

        return services;
    }
}
