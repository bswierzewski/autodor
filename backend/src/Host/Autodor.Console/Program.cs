using Autodor.Console;
using Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;
using CommandLine;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var exitCode = await Parser.Default.ParseArguments<Options>(args)
    .MapResult(
        async (Options opts) => await RunApplicationAsync(opts),
        errors => Task.FromResult(1)
    );

Environment.Exit(exitCode);

async Task<int> RunApplicationAsync(Options options)
{
    var configuration = Extensions.CreateConfiguration();
    using IHost host = configuration.CreateHostBuilder(options).Build();

    Extensions.ConfigureLogging();

    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var mediator = services.GetRequiredService<ISender>();

        Log.Information("Console application started with operation: {Operation}", options.Operation);
        Log.Information("Date range: {From} - {To}", options.From, options.To);

        switch (options.Operation.ToLowerInvariant())
        {
            case "invoices":
                await CreateBulkInvoicesAsync(mediator, options.From, options.To);
                break;
            default:
                Log.Warning("Unknown operation: {Operation}", options.Operation);
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