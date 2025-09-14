using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Autodor.Modules.Orders.Infrastructure.Persistence;
using Autodor.Modules.Products.Infrastructure.Persistence;
using Autodor.Tests.E2E.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;

namespace Autodor.Tests.E2E.Core.Factories;

/// <summary>
/// Test web application factory that manages PostgreSQL container and configures separate databases for each module.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithUsername("testuser")
            .WithPassword("testpassword")
            .WithDatabase("postgres")
            .Build();

    private Respawner _respawner = null!;
    private NpgsqlConnection _connection = null!;

    /// <summary>
    /// Initializes PostgreSQL container and database infrastructure.
    /// </summary>
    public async Task InitializeAsync()
    {
        // 1. Start the PostgreSQL container. The database specified in 'WithDatabase' will be created automatically.
        await _container.StartAsync();

        // 2. Create a single connection to be used for migrations and resetting the database.
        _connection = new NpgsqlConnection(_container.GetConnectionString());
        await _connection.OpenAsync();

        // 3. Apply all Entity Framework Core migrations.
        // We can now call MigrateAsync() because the database already exists.
        using (var scope = Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            await serviceProvider.GetRequiredService<ContractorsDbContext>().Database.MigrateAsync();
            await serviceProvider.GetRequiredService<OrdersDbContext>().Database.MigrateAsync();
            await serviceProvider.GetRequiredService<ProductsDbContext>().Database.MigrateAsync();
        }
        // 4. Initialize a single Respawner for the shared database.
        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore = ["__EFMigrationsHistory"],
            WithReseed = true
        });
    }

    /// <summary>
    /// Disposes the database container and connection after all tests are finished.
    /// </summary>
    public new async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _container.DisposeAsync();
        await base.DisposeAsync();
    }

    /// <summary>
    /// Resets the database to a clean state. This should be called before each test execution.
    /// </summary>
    public async Task ResetDatabasesAsync()
    {
        // The connection is already open, so we can reset efficiently.
        await _respawner.ResetAsync(_connection);
    }

    /// <summary>
    /// Configures the web host for testing with separate databases per module.
    /// </summary>
    /// <param name="builder">The web host builder to configure.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Register all DbContexts to use the same connection string pointing to the single test database.
            var connectionString = _container.GetConnectionString();

            services
                .ReplaceDbContext<ContractorsDbContext>(connectionString)
                .ReplaceDbContext<OrdersDbContext>(connectionString)
                .ReplaceDbContext<ProductsDbContext>(connectionString);
        });
    }
}