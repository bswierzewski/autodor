using Autodor.Tests.E2E.Core.Collections;
using Autodor.Tests.E2E.Core.Factories;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Autodor.Tests.E2E.Core;

[Collection(nameof(E2ECollection))]
public abstract class TestBase : IAsyncLifetime
{
    private readonly TestWebApplicationFactory _factory;

    protected HttpClient Client { get; }

    protected IServiceProvider Services { get; }

    protected TestBase(TestWebApplicationFactory factory)
    {
        _factory = factory;
        Client = _factory.CreateClient();
        Services = _factory.Services;
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabasesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected IServiceScope CreateScope()
    {
        return Services.CreateScope();
    }
}