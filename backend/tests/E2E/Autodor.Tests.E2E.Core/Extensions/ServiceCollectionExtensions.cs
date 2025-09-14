using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Autodor.Tests.E2E.Core.Extensions
{
    /// <summary>
    /// Provides extension methods for IServiceCollection to simplify test setup.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="TContext">The type of the DbContext to replace.</typeparam>
        /// <param name="services">The IServiceCollection to configure.</param>
        /// <param name="connectionString">The connection string for the test database.</param>
        /// <returns>The IServiceCollection for chaining.</returns>
        public static IServiceCollection ReplaceDbContext<TContext>(this IServiceCollection services, string connectionString)
            where TContext : DbContext
        {
            // 1. Remove the old DbContextOptions registration from the main application.
            services.RemoveAll<DbContextOptions<TContext>>();

            // 2. Remove the old DbContext registration itself. This is a safety measure
            //    to ensure no previous registration interferes with the test setup.
            services.RemoveAll<TContext>();

            // 3. Add the new DbContext registration pointing to the test database.
            //    It dynamically finds the correct migrations assembly based on the DbContext's type.
            services.AddDbContext<TContext>(options =>
                options.UseNpgsql(connectionString,
                    b => b.MigrationsAssembly(typeof(TContext).Assembly.FullName)));

            return services;
        }
    }
}
