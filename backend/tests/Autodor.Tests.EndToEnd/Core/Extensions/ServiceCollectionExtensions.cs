using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Autodor.Tests.E2E.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ReplaceDbContext<TContext>(this IServiceCollection services, string connectionString)
            where TContext : DbContext
        {
            services.RemoveAll<DbContextOptions<TContext>>();
            services.RemoveAll<TContext>();

            services.AddDbContext<TContext>(options =>
                options.UseNpgsql(connectionString,
                    b => b.MigrationsAssembly(typeof(TContext).Assembly.FullName)));

            return services;
        }
    }
}