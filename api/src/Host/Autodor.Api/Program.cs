using Autodor.Api;
using Autodor.Modules.Contractors;
using Autodor.Modules.Invoicing;
using Autodor.Modules.Orders;
using Autodor.Modules.Products;
using Autodor.ServiceDefaults;
using Autodor.Shared.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Wolverine;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

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
    opts.Discovery.IncludeAssembly(typeof(ContractorsModuleExtensions).Assembly);
    opts.Discovery.IncludeAssembly(typeof(InvoicingModuleExtensions).Assembly);
    opts.Discovery.IncludeAssembly(typeof(ProductsModuleExtensions).Assembly);
    opts.Discovery.IncludeAssembly(typeof(OrdersModuleExtensions).Assembly);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapWolverineEndpoints();

app.Run();