using Autodor.Api;
using Autodor.Modules.Contractors;
using Autodor.Modules.Contractors.Infrastructure.Database;
using Autodor.Modules.Invoicing;
using Autodor.Modules.Invoicing.Infrastructure.Database;
using Autodor.Modules.Orders;
using Autodor.Modules.Orders.Infrastructure.Database;
using Autodor.Modules.Products;
using Autodor.Modules.Products.Infrastructure.Database;
using Autodor.ServiceDefaults;
using Autodor.Shared.Core.Interfaces;
using Autodor.Shared.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Scalar.AspNetCore;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with Aspire and Wolverine integration
builder.Host.UseSerilog(logging =>
{
    logging.AddConsole();
    logging.AddOpenTelemetry();
    logging.AddFile();
});

builder.AddServiceDefaults();

builder.Services.AddContractorsModule(builder.Configuration);
builder.Services.AddInvoicingModule(builder.Configuration);
builder.Services.AddOrdersModule(builder.Configuration);
builder.Services.AddProductsModule(builder.Configuration);

// Dummy implementation of IUserContext for demonstration purposes
// Remove after add IHttpContextAccessor and real user context implementation
builder.Services.TryAddScoped<IUserContext, DummyUserContext>();

builder.Host.UseWolverine(opts =>
{
    // Tell Wolverine that when you have more than one handler for the same
    // message type, they should be executed separately and automatically
    // "stuck" to separate local queues
    opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;

    // Not 100% necessary for "modular monoliths", but this makes the Wolverine durable
    // inbox/outbox feature a lot easier to use and DRYs up your message handlers
    opts.Policies.AutoApplyTransactions();

    // Add all of the message handlers, etc. from the modules
    opts.Discovery.IncludeAssembly(typeof(ContractorsModuleExtensions).Assembly);
    opts.Discovery.IncludeAssembly(typeof(InvoicingModuleExtensions).Assembly);
    opts.Discovery.IncludeAssembly(typeof(ProductsModuleExtensions).Assembly);
    opts.Discovery.IncludeAssembly(typeof(OrdersModuleExtensions).Assembly);
});

// Add services to the container.
builder.Services.AddWolverineHttp();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapWolverineEndpoints();

await app.MigrateDatabaseAsync<ContractorsDbContext>();
await app.MigrateDatabaseAsync<InvoicingDbContext>();
await app.MigrateDatabaseAsync<ProductsDbContext>();
await app.MigrateDatabaseAsync<OrdersDbContext>();

app.Run();