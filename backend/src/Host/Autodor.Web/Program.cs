using Autodor.Modules.Contractors.Infrastructure;
using Autodor.Modules.Invoicing.Infrastructure;
using Autodor.Modules.Orders.Infrastructure;
using Autodor.Modules.Products.Infrastructure;
using BuildingBlocks.Infrastructure.Extensions;
using DotNetEnv;
using Serilog;

if (File.Exists(".env"))
    Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

builder.Services.AddCors();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddUserContext();
builder.Services.AddProblemDetails(options => options.AddCustomConfiguration(builder.Environment));
builder.Services.AddOpenApi(options => options.AddProblemDetailsSchemas());

builder.Services.RegisterModules(builder.Configuration, [
    new ContractorsModule(),
    new InvoicingModule(),
    new OrdersModule(),
    new ProductsModule()
    ]);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseCors(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
}

app.UseAuthentication();
app.UseAuthorization();

app.UseModules(builder.Configuration);

await app.Services.InitModules();
await app.RunAsync();
