using Autodor.Modules.Contractors.Infrastructure;
using Autodor.Modules.Invoicing.Infrastructure;
using Autodor.Modules.Orders.Infrastructure;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SharedKernel.Application;
using SharedKernel.Infrastructure;

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

    public static IHostBuilder CreateHostBuilder(this IConfiguration configuration)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // First to ensure user services are available for other modules

                services.AddSharedKernelApplication();
                services.AddSharedKernelInfrastructure();

                services.AddContractors(configuration);
                services.AddProducts(configuration);
                services.AddOrders(configuration);
                services.AddInvoicing();

                services.AddOptions<Options>()
                    .Configure(opt => Parser.Default.ParseArguments(() => opt, Environment.GetCommandLineArgs()));

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