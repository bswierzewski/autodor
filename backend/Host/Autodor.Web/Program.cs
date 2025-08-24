using Autodor.Modules.Contractors.Infrastructure;
using Autodor.Modules.Products.Infrastructure;
using Autodor.Modules.Orders.Infrastructure;
using Autodor.Modules.Invoicing.Infrastructure;
using Autodor.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Rejestracja MediatR dla wszystkich assembly
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

// Rejestracja modułów
builder.Services.AddContractors(builder.Configuration);  // z DbContext
builder.Services.AddProducts(builder.Configuration);     // bez DbContext (SOAP only)
builder.Services.AddOrders(builder.Configuration);       // z DbContext (ExcludedOrders)
builder.Services.AddInvoicingModule();                   // bez DbContext

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Mapowanie endpoints
app.MapContractorsEndpoints();
app.MapOrdersEndpoints();
app.MapProductsEndpoints();
app.MapInvoicingEndpoints();

app.Run();