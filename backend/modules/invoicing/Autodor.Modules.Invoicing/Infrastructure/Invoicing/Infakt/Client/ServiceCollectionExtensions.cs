using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client.Handlers;
using Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Options;
using BuildingBlocks.Infrastructure.Wolverine.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Invoicing.Infakt.Client;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInFaktHttpClient(this IServiceCollection services)
    {
        services.AddTransient<InFaktAuthenticationHandler>();
        services.AddTransient<InFaktErrorHandler>();
        services.AddWolverineRefitClient<IInFaktHttpClient>()
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
