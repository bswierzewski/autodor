using Autodor.Modules.Invoicing.Application.Abstractions;
using Autodor.Modules.Invoicing.Infrastructure.Factories;
using Autodor.Modules.Invoicing.Infrastructure.Services;
using Autodor.Shared.Application.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddInvoicingModule(this IServiceCollection services)
    {
        // Rejestracja shared behaviors
        services.AddSharedApplicationBehaviors(Autodor.Modules.Invoicing.Application.AssemblyReference.Assembly);

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