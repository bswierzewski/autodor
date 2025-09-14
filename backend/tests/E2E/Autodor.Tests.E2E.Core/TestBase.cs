using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Autodor.Tests.E2E.Core.Factories;

namespace Autodor.Tests.E2E.Core;

/// <summary>
/// Base class for all E2E tests providing shared test infrastructure and database management.
/// </summary>
[Collection("E2E Test Collection")]
public abstract class TestBase : IAsyncLifetime
{
    private readonly TestWebApplicationFactory _factory;

    /// <summary>
    /// HTTP client configured for testing the application.
    /// </summary>
    protected HttpClient Client { get; }

    /// <summary>
    /// Service provider for accessing application services in tests.
    /// </summary>
    protected IServiceProvider Services { get; }

    /// <summary>
    /// Initializes a new instance of the TestBase class.
    /// </summary>
    /// <param name="factory">The application factory injected by xUnit (via IClassFixture).</param>
    protected TestBase(TestWebApplicationFactory factory)
    {
        _factory = factory;
        Client = _factory.CreateClient();
        Services = _factory.Services;
    }

    /// <summary>
    /// Initializes the test by resetting the database. This runs before each test.
    /// </summary>
    public async Task InitializeAsync()
    {
        await _factory.ResetDatabasesAsync();
    }

    /// <summary>
    /// Performs cleanup after each test. No action is required here in this setup.
    /// </summary>
    public Task DisposeAsync() => Task.CompletedTask;

    /// <summary>
    /// Creates a service scope for resolving scoped services.
    /// Remember to dispose of the scope, preferably with a 'using' statement.
    /// </summary>
    /// <returns>A service scope that must be disposed after use.</returns>
    protected IServiceScope CreateScope()
    {
        return Services.CreateScope();
    }
}