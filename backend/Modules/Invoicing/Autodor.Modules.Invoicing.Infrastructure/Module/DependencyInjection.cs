using System.Reflection;
using Autodor.Modules.Invoicing.Application.Module;
using Autodor.Modules.Invoicing.Infrastructure.Factories;
using Autodor.Modules.Invoicing.Infrastructure.Services;
using BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing.Infrastructure.Module;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register MediatR for any infrastructure-level handlers or behaviors
        // This enables infrastructure event handling and cross-cutting concerns
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Rejestracja serwisów Infrastructure
        services.AddScoped<MockInvoiceService>();
        services.AddScoped<MockPdfGeneratorService>();
        services.AddScoped<InvoiceServiceFactory>();

        // Rejestracja fabryk jako fabryczne metody
        services.AddScoped(provider => 
        {
            var factory = provider.GetRequiredService<InvoiceServiceFactory>();
            return factory.CreateInvoiceService();
        });

        services.AddScoped(provider => 
        {
            var factory = provider.GetRequiredService<InvoiceServiceFactory>();
            return factory.CreatePdfGeneratorService();
        });

        return services;
    }
}