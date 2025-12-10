using Microsoft.Extensions.DependencyInjection;

namespace Autodor.Modules.Invoicing.Infrastructure.Services.IFirma.Clients.Extensions
{
    /// <summary>
    /// Extension methods for registering IFirma HTTP client
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers IFirma HTTP client with HMAC-SHA1 authentication
        /// </summary>
        public static IServiceCollection AddIFirmaHttpClient(this IServiceCollection services)
        {
            services.AddTransient<IFirmaAuthenticationHandler>();
            services.AddHttpClient<IFirmaHttpClient>((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri("https://www.ifirma.pl/");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddHttpMessageHandler<IFirmaAuthenticationHandler>();

            return services;
        }
    }
}
