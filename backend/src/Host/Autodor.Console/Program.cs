using Autodor.Console.Services;
using Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;
using BuildingBlocks.Abstractions.Abstractions;
using BuildingBlocks.Infrastructure.Extensions;
using DotNetEnv;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

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

using IHost host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(configBuilder => configBuilder.AddConfiguration(configuration))
    .UseSerilog((context, services, config) =>
    {
        config
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Application", "Autodor.Console");
    })
    .ConfigureServices(services =>
    {
        services.RegisterModules(configuration);

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IUserContext, ConsoleUser>();

        services.AddOptions<Options>()
            .Configure(opts =>
            {
                opts.From = DateTime.Parse(configuration["from"] ?? throw new InvalidOperationException("Missing 'from' parameter"));
                opts.To = DateTime.Parse(configuration["to"] ?? throw new InvalidOperationException("Missing 'to' parameter"));
                opts.Operation = configuration["operation"] ?? "invoices";
            });
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
    try
    {
        var invoiceStatuses = await mediator.Send(command);

        var invoicesList = invoiceStatuses.ToList();
        Log.Information("Successfully created {Count} invoices", invoicesList.Count);

        foreach (var (nip, success) in invoicesList)
        {
            Log.Information("Invoice for NIP {NIP}: {Status}", nip, success ? "Success" : "Failed");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to create bulk invoices: {Message}", ex.Message);
    }
}

public partial class Program { }