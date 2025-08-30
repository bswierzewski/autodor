using Autodor.Modules.Contractors.Infrastructure;
using Autodor.Modules.Invoicing.Infrastructure;
using Autodor.Modules.Orders.Infrastructure;
using Autodor.Web.Endpoints;
using Autodor.Web.Services;
using SharedKernel.Application;
using SharedKernel.Application.Interfaces;
using SharedKernel.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSharedKernelApplication();
builder.Services.AddSharedKernelInfrastructure();

// Rejestracja IUser serwisu
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
app.MapProductsEndpoints();
app.MapInvoicingEndpoints();

app.Run();