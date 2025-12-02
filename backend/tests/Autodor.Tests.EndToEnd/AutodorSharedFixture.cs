using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Tests.Authentication;
using Shared.Infrastructure.Tests.Core;
using Shared.Infrastructure.Tests.Infrastructure.Containers;

namespace Autodor.Tests.EndToEnd;

/// <summary>
/// Shared test fixture for Autodor module end-to-end tests.
/// Provides shared infrastructure (PostgreSQL container, token provider) across all test classes.
/// Each test class creates its own TestContext with custom service configuration (mocks).
/// </summary>
/// <remarks>
/// This fixture is created ONCE per test collection and shared across all test classes.
/// It provides:
/// - PostgreSQL container (started once, shared)
/// - Token provider (single instance with built-in cache)
/// - Bootstrap context for running migrations once
/// 
/// Individual test classes create their own TestContext with:
/// - Custom mock configurations
/// - Own WebApplicationFactory instance
/// - WithoutModuleInitialization() to skip migrations (already run by fixture)
/// </remarks>
public class AutodorSharedFixture : IAsyncLifetime
{
    private TestContext _bootstrapContext = null!;

    /// <summary>
    /// Gets the shared PostgreSQL container.
    /// Use this in test classes with TestContextBuilder.WithContainer().
    /// </summary>
    public PostgreSqlTestContainer Container { get; } = new();

    /// <summary>
    /// Gets the shared token provider instance.
    /// Single instance ensures token caching works across all test classes.
    /// </summary>
    public ITokenProvider TokenProvider { get; private set; } = null!;

    /// <summary>
    /// Gets the test user options (email, password) from configuration.
    /// </summary>
    public TestUserOptions TestUser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        // Start PostgreSQL container (once for all tests)
        await Container.StartAsync();

        // Create bootstrap context to run migrations and get configuration
        _bootstrapContext = await TestContext.CreateBuilder<Program>()
            .WithContainer(Container)
            .WithServices((services, configuration) =>
            {
                services.ConfigureOptions<TestUserOptions>(configuration);
                services.AddSingleton<ITokenProvider, SupabaseTokenProvider>();
            })
            .BuildAsync();

        // Get shared instances
        TestUser = _bootstrapContext.GetRequiredService<IOptions<TestUserOptions>>().Value;
        TokenProvider = _bootstrapContext.GetRequiredService<ITokenProvider>();
    }

    public async Task DisposeAsync()
    {
        if (_bootstrapContext != null)
        {
            await _bootstrapContext.DisposeAsync();
        }

        await Container.StopAsync();
    }
}

/// <summary>
/// xUnit collection definition for sharing the AutodorSharedFixture across tests.
/// All tests with [Collection("Autodor")] share a single PostgreSQL container and token provider.
/// </summary>
[CollectionDefinition("Autodor")]
public class AutodorCollection : ICollectionFixture<AutodorSharedFixture>
{
}
