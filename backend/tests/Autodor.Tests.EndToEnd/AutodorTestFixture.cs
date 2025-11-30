using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.Extensions;
using Shared.Infrastructure.Tests.Authentication;
using Shared.Infrastructure.Tests.Core;

namespace Autodor.Tests.EndToEnd;

/// <summary>
/// Test fixture for Autodor module end-to-end tests.
/// Provides shared test infrastructure with PostgreSQL container and test authentication.
/// Uses the Autodor.Web project as the test host.
/// </summary>
public class AutodorTestFixture : IAsyncLifetime
{
    public TestContext Context { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Context = await TestContext.CreateBuilder<Program>()
            .WithPostgreSql()
            .WithServices((services, configuration) =>
            {
                // Register test user credentials from environment variables
                services.ConfigureOptions<TestUserOptions>(configuration);

                // Register Supabase token provider for authentication
                services.AddSingleton<ITokenProvider, SupabaseTokenProvider>();
            })
            .BuildAsync();
    }

    public Task DisposeAsync() => Context?.DisposeAsync().AsTask() ?? Task.CompletedTask;
}

/// <summary>
/// xUnit collection definition for sharing the AutodorTestFixture across tests.
/// This allows all tests in this collection to share a single test context, container, and database.
/// </summary>
[CollectionDefinition("Autodor")]
public class AutodorCollection : ICollectionFixture<AutodorTestFixture>
{
}
