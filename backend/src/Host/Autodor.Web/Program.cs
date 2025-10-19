using Autodor.Modules.Contractors.Infrastructure;
using Autodor.Modules.Invoicing.Infrastructure;
using Autodor.Modules.Orders.Infrastructure;
using Autodor.Modules.Products.Infrastructure;
using Autodor.Web.Endpoints;
using BuildingBlocks.Modules.Users.Web;
using BuildingBlocks.Modules.Users.Web.Extensions;
using BuildingBlocks.Modules.Users.Web.Module;
using BuildingBlocks.Modules.Users.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja usług podstawowych
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpContextAccessor();

// Rejestracja modułu Users
builder.Services.AddUsers(builder.Configuration);

// Konfiguracja autentykacji z Clerk
builder.Services.AddClerkOptions(builder.Configuration);
builder.Services
    .AddAuthentication()
    .AddClerkJwtBearer();

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

app.UseAuthentication();
app.UseAuthorization();

// Mapowanie endpoints
app.MapContractorsEndpoints();
app.MapOrdersEndpoints();
app.MapInvoicingEndpoints();
app.MapUsersEndpoints();

app.Run();

// Make the Program class accessible for integration tests
public partial class Program { }
