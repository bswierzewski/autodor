using Autodor.Shared.Core.Interfaces;
using Autodor.Shared.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;

namespace Autodor.Shared.Infrastructure.Extensions;

public static class ModuleExtensions
{
    public static ModuleBuilder AddModule(this IServiceCollection services, IConfiguration configuration, string moduleName)
        => new(services, configuration, moduleName);
}

public sealed class ModuleBuilder(IServiceCollection services, IConfiguration configuration, string moduleName)
{
    public IServiceCollection Services { get; } = services;
    public IConfiguration Configuration { get; } = configuration;
    public string ModuleName { get; } = moduleName;
    public string SchemaName { get; } = moduleName.ToLowerInvariant();

    public ModuleBuilder AddPostgres<TDbContext>()
        where TDbContext : DbContext
    {
        Services.AddKeyedSingleton(ModuleName, (sp, key) =>
        {
            // Get connection string from standard location (works with Aspire, Docker Compose, and appsettings)
            var connectionString = Configuration.GetConnectionString(SchemaName);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Connection string '{SchemaName}' not found. ");

            return new NpgsqlDataSourceBuilder(connectionString)
                .EnableDynamicJson()
                .Build();
        });

        Services.TryAddScoped<AuditableEntityInterceptor>();

        Services.AddDbContext<TDbContext>((sp, options) =>
        {
            var dataSource = sp.GetRequiredKeyedService<NpgsqlDataSource>(ModuleName);
            var auditableInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

            options.UseNpgsql(dataSource, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", SchemaName);
            });

            options.AddInterceptors(auditableInterceptor);

        });

        return this;
    }

    public ModuleBuilder AddOptions(Action<ModuleBuilder> configure)
    {
        configure(this);

        return this;
    }

    public ModuleBuilder ConfigureOptions<T>() where T : class, IOptions
    {
        Services.Configure<T>(Configuration.GetSection(T.SectionName));

        return this;
    }

    public IServiceCollection Build() => Services;
}