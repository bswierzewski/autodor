using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Handlers;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInFaktHttpClient(this IServiceCollection services)
    {
        services.AddTransient<InFaktAuthenticationHandler>();
        services.AddTransient<InFaktErrorHandler>();
        services.AddRefitClient<IInFaktHttpClient>()
            .ConfigureHttpClient((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<InFaktOptions>>().Value;
                var baseUri = options.BaseUrl.TrimEnd('/') + "/";
                client.BaseAddress = new Uri(baseUri);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<InFaktAuthenticationHandler>()
            .AddHttpMessageHandler<InFaktErrorHandler>();

        return services;
    }
}
