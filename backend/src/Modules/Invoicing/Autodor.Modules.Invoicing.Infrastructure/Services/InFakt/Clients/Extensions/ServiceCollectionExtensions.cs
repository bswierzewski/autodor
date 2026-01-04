using Autodor.Modules.Invoicing.Application.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInFaktHttpClient(this IServiceCollection services)
        {
            services.AddTransient<InFaktAuthenticationHandler>();
            services.AddHttpClient<InFaktHttpClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<InFaktOptions>>().Value;
                var baseUri = options.BaseUrl.TrimEnd('/') + "/";
                client.BaseAddress = new Uri(baseUri);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<InFaktAuthenticationHandler>();

            return services;
        }
    }
}
