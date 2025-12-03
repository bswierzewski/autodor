using Autodor.Console;
using Autodor.Console.Services;
using Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;
using DotNetEnv;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Shared.Abstractions.Authorization;
using Shared.Abstractions.Modules;
using Serilog;

// Load environment variables from .env file BEFORE creating builder
if (File.Exists(".env"))
    Env.Load();

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddCommandLine(args, new Dictionary<string, string>
    {
        { "-f", "from" },
        { "--from", "from" },
        { "-t", "to" },
        { "--to", "to" },
        { "-o", "operation" },
        { "--operation", "operation" }
    })
    .Build();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        path: Path.Combine("Logs", "log.txt"),
        rollingInterval: RollingInterval.Day
    )
    .CreateLogger();

using IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(configBuilder => configBuilder.AddConfiguration(configuration))
    .ConfigureServices(services =>
    {
        services.RegisterModules(configuration);

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IUser, ConsoleUser>();

        services.AddOptions<Options>()
            .Configure(opts =>
            {
                opts.From = DateTime.Parse(configuration["from"] ?? throw new InvalidOperationException("Missing 'from' parameter"));
                opts.To = DateTime.Parse(configuration["to"] ?? throw new InvalidOperationException("Missing 'to' parameter"));
                opts.Operation = configuration["operation"] ?? "invoices";
            });

        services.AddSerilog();
    })
    .Build();

var exitCode = await RunApplicationAsync();
Environment.Exit(exitCode);

async Task<int> RunApplicationAsync()
{
    try
    {
        await host.Services.InitModules();

        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var options = services.GetRequiredService<IOptions<Options>>();
        var mediator = services.GetRequiredService<ISender>();

        var opts = options.Value;
        Log.Information("Console application started with operation: {Operation}", opts.Operation);
        Log.Information("Date range: {From} - {To}", opts.From, opts.To);

        switch (opts.Operation.ToLowerInvariant())
        {
            case "invoices":
                await CreateBulkInvoicesAsync(mediator, opts.From, opts.To);
                break;
            default:
                Log.Warning("Unknown operation: {Operation}", opts.Operation);
                break;
        }

        return 0;
    }
    catch (Exception e)
    {
        Log.Logger.Error(e, "An error occurred during console operation: {Message}", e.Message);
        return 1;
    }
}

static async Task CreateBulkInvoicesAsync(ISender mediator, DateTime from, DateTime to)
{
    Log.Information("Creating bulk invoices for date range: {From} - {To}", from, to);

    var command = new CreateBulkInvoicesCommand(from, to);
    var invoiceNumbers = await mediator.Send(command);

    var invoicesList = invoiceNumbers.ToList();
    Log.Information("Successfully created {Count} invoices", invoicesList.Count);

    foreach (var invoiceNumber in invoicesList)
    {
        Log.Information("Created invoice: {InvoiceNumber}", invoiceNumber);
    }
}

// [GenerateModuleRegistry] triggers source generator to create ModuleRegistry class
[GenerateModuleRegistry]
public partial class Program { }