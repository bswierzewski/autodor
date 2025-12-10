using Autodor.Modules.Invoicing.Application.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.InFakt.Clients.Extensions
{
    /// <summary>
    /// Extension methods for registering InFakt HTTP client
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers InFakt HTTP client with API key authentication handler
        /// </summary>
        public static IServiceCollection AddInFaktHttpClient(this IServiceCollection services)
        {
            services.AddTransient<InFaktAuthenticationHandler>();
            services.AddHttpClient<InFaktHttpClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<InFaktOptions>>().Value;
                client.BaseAddress = new Uri(options.ApiUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<InFaktAuthenticationHandler>();

            return services;
        }
    }
}
