using Autodor.Modules.Contractors.Infrastructure;
using Autodor.Modules.Products.Infrastructure;
using Autodor.Modules.Orders.Infrastructure;
using Autodor.Modules.Invoicing.Infrastructure;
using Autodor.Modules.Users.Infrastructure;
using CommandLine;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

var configuration = CreateConfigurationBuilder().Build();

using IHost host = CreateHostBuilder().Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine("Logs", "log.txt"),
        rollingInterval: RollingInterval.Day
        )
    .CreateLogger();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var options = services.GetRequiredService<IOptions<Options>>();
    var mediator = services.GetRequiredService<ISender>();

    // Przykład wywołania konkretnej operacji
    Log.Information("Console application started with options: {From} - {To}", options.Value.From, options.Value.To);

    // Tutaj można dodać konkretne komendy MediatR
    // await mediator.Send(new SomeCommand());
}
catch (Exception e)
{
    Log.Logger.Error(e, e.Message);
}

IHostBuilder CreateHostBuilder()
{
    return Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        // Rejestracja modułów (USERS FIRST!)
        services.AddUsersForConsole();     // Users Module FIRST!

        services.AddContractors(configuration);  // z DbContext
        services.AddProducts(configuration);     // bez DbContext (SOAP only)
        services.AddOrders(configuration);       // z DbContext (ExcludedOrders)
        services.AddInvoicingModule();           // bez DbContext

        services.AddOptions<Options>()
            .Configure(opt => Parser.Default.ParseArguments(() => opt, Environment.GetCommandLineArgs()));

        services.AddSerilog();
    });
}

IConfigurationBuilder CreateConfigurationBuilder()
{
    return new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
        .AddEnvironmentVariables();
}