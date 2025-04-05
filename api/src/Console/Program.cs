using Application.Common.Interfaces;
using Application.Interfaces;
using Application.Invoices.Commands.CreateAllInvoices;
using CommandLine;
using Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

var configuration = CreateConfigurationBuilder()
    .Build();

using IHost host = CreateHostBuilder()
    .Build();

/// Logger configuration
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

    await services.GetRequiredService<ISender>()
        .Send(new CreateAllInvoicesCommand
        {
            DateFrom = options.Value.From,
            DateTo = options.Value.To
        });
}
catch (Exception e)
{
    Log.Logger.Error(e, e.Message);

    await services.GetRequiredService<ISendGridService>().SendEmail(["swierzewski.bartosz@gmail.com"], "Error", e.Message);
}

IHostBuilder CreateHostBuilder()
{
    return Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddApplicationServices(configuration);
        services.AddInfrastructureServices(configuration);

        services.AddOptions<Options>()
            .Configure(opt => Parser.Default.ParseArguments(() => opt, Environment.GetCommandLineArgs()));

        services.AddSerilog();

        services.AddScoped<IUser, User>();
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