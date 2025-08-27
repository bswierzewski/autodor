using Autodor.Modules.Contractors.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Contractors.Infrastructure.Services;

public class ContractorsMigrationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ContractorsMigrationService> _logger;

    public ContractorsMigrationService(IServiceProvider serviceProvider, ILogger<ContractorsMigrationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContractorsDbContext>();

        try
        {
            _logger.LogInformation("Starting Contractors database migration...");
            await context.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Contractors database migration completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during Contractors database migration");
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}