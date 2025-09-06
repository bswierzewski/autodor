using System.Reflection;
using Autodor.Modules.Invoicing.Application;
using Autodor.Modules.Invoicing.Infrastructure.Factories;
using Autodor.Modules.Invoicing.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInvoicing(this IServiceCollection services)
    {
        services.AddApplication();
        services.AddInfrastructure();
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Rejestracja MediatR
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