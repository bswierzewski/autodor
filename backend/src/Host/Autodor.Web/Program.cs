using Autodor.Modules.Contractors.Infrastructure;
using Autodor.Modules.Invoicing.Infrastructure;
using Autodor.Modules.Orders.Infrastructure;
using Autodor.Modules.Products.Infrastructure;
using Autodor.Web.Endpoints;
using Autodor.Web.Services;
using BuildingBlocks.Application.Abstractions;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja IUser serwisu
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<IUser, StaticUserService>();

builder.Services.AddContractors(builder.Configuration);
builder.Services.AddProducts(builder.Configuration, options =>
{
    options.AddSynchronization();
});
builder.Services.AddOrders(builder.Configuration);
builder.Services.AddInvoicing();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Mapowanie endpoints
app.MapContractorsEndpoints();
app.MapOrdersEndpoints();
app.MapInvoicingEndpoints();

app.Run();

// Make the Program class accessible for integration tests
public partial class Program { }