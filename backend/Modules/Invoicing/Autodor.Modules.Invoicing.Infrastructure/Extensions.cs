using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Infrastructure.Factories;
using Autodor.Modules.Invoicing.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Autodor.Modules.Invoicing.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInvoicingModule(this IServiceCollection services)
    {
        // Rejestracja MediatR dla Application layer
        services.AddMediatR(cfg => 
        {
            cfg.RegisterServicesFromAssembly(Application.AssemblyReference.Assembly);
        });

        // Rejestracja serwisów Infrastructure
        services.AddScoped<MockInvoiceService>();
        services.AddScoped<MockPdfGeneratorService>();
        services.AddScoped<InvoiceServiceFactory>();

        // Rejestracja fabryk jako fabryczne metody
        services.AddScoped<IInvoiceService>(provider => 
        {
            var factory = provider.GetRequiredService<InvoiceServiceFactory>();
            return factory.CreateInvoiceService();
        });

        services.AddScoped<IPdfGeneratorService>(provider => 
        {
            var factory = provider.GetRequiredService<InvoiceServiceFactory>();
            return factory.CreatePdfGeneratorService();
        });

        return services;
    }
}