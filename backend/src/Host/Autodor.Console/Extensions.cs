using Autodor.Console.Services;
using Autodor.Modules.Contractors.Infrastructure;
using Autodor.Modules.Invoicing.Infrastructure;
using Autodor.Modules.Orders.Infrastructure;
using Autodor.Modules.Products.Infrastructure;
using BuildingBlocks.Application.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Autodor.Console;

public static class Extensions
{
    public static IConfiguration CreateConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static IHostBuilder CreateHostBuilder(this IConfiguration configuration, Options options)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Register user service first to ensure it's available for other modules
                services.AddScoped<IUser, ConsoleUser>();

                services.AddSingleton(TimeProvider.System);

                services.AddContractors(configuration);
                services.AddProducts(configuration);
                services.AddOrders(configuration);
                services.AddInvoicing();

                // Register parsed options as singleton
                services.AddSingleton(options);

                services.AddSerilog();
            });
    }

    public static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                path: Path.Combine("Logs", "log.txt"),
                rollingInterval: RollingInterval.Day
            )
            .CreateLogger();
    }
}