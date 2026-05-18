using Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.IFirma.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIFirmaHttpClient(this IServiceCollection services)
    {
        services.AddTransient<IFirmaAuthenticationHandler>();
        services.AddHttpClient<IFirmaHttpClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<IFirmaOptions>>().Value;
            var baseUri = options.BaseUrl.TrimEnd('/') + "/";
            client.BaseAddress = new Uri(baseUri);
            client.Timeout = TimeSpan.FromSeconds(30);
        })
        .AddHttpMessageHandler<IFirmaAuthenticationHandler>();

        return services;
    }
}
