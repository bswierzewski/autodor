using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Products.Infrastructure.Persistence;

public class ProductsMigrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProductsMigrationService> _logger;

    public ProductsMigrationService(IServiceProvider serviceProvider, ILogger<ProductsMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();

        try
        {
            _logger.LogInformation("Starting Products database migration...");
            await context.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Products database migration completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Products database migration");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}