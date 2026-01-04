using Autodor.Modules.Invoicing.Application;
using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Application.Options;
using Autodor.Modules.Invoicing.Domain;
using Autodor.Modules.Invoicing.Domain.Enums;
using Autodor.Modules.Invoicing.Infrastructure.Endpoints;
using Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions;
using Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Extensions;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing.Infrastructure;

public class InvoicingModule : IModule
{
    public string Name => Module.Name;

    public void Register(IServiceCollection services, IConfiguration configuration)
    {
        services.AddModule(configuration, Name)
            .AddOptions((svc, config) =>
            {
                svc.ConfigureOptions<InvoicingOptions>(config);
                svc.ConfigureOptions<InFaktOptions>(config);
                svc.ConfigureOptions<IFirmaOptions>(config);
            })
            .AddCQRS(typeof(ApplicationAssembly).Assembly, typeof(InfrastructureAssembly).Assembly)
            .Build();

        services.AddIFirmaHttpClient();
        services.AddInFaktHttpClient();

        services.AddKeyedScoped<IInvoiceService, Services.InFakt.Services.InFaktInvoiceService>(InvoiceProvider.InFakt);
        services.AddKeyedScoped<IInvoiceService, Services.IFirma.Services.IFirmaInvoiceService>(InvoiceProvider.IFirma);
    }

    public void Use(IApplicationBuilder app, IConfiguration configuration)
    {
        var endpoints = (IEndpointRouteBuilder)app;

        endpoints.MapInvoicingEndpoints();
    }

    public Task Initialize(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

}
