using Autodor.API;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Hosting.Enums;
using BuildingBlocks.Hosting.Extensions;
using BuildingBlocks.Infrastructure.Exceptions.Handlers;
using BuildingBlocks.Infrastructure.Exceptions.Extensions;
using BuildingBlocks.Infrastructure.Identity.Extensions;
using BuildingBlocks.Infrastructure.OpenApi;
using BuildingBlocks.Infrastructure.Serilog.Extensions;
using BuildingBlocks.Infrastructure.Wolverine.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var executionMode = builder.GetExecutionMode();

// Add Aspire service defaults
if (executionMode != ApplicationExecutionMode.OpenApi)
    builder.AddServiceDefaults();

// Configure Serilog with Aspire and Wolverine integration
builder.Host.UseSerilog(logging =>
{
    logging.AddConsole();
    logging.AddOpenTelemetry();
    logging.AddFile();
});

// Add global exception handling
builder.Services.AddProblemDetails(options =>
{
    options.AddDiagnosticInformation();
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddIdentity(builder.Configuration);

builder.Services.AddOpenApi(options =>
{
    options.AddProblemDetailsResponses();
    options.AddBearerSecurityScheme();
});

builder.Services.ConfigureModules(builder.Configuration, out IModule[] modules);

if (executionMode == ApplicationExecutionMode.OpenApi)
    builder.ConfigureOpenApiWolverine(modules);
else
    builder.ConfigureWolverine(modules);

// Configure CORS
builder.Services.AddCors();

var app = builder.Build();

if (executionMode != ApplicationExecutionMode.OpenApi)
    await app.InitializeModulesAsync(modules);

// Use global exception handler
app.UseExceptionHandler();

// Serve static files and default documents for SPA support
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// Map Aspire default endpoints (health checks, etc.)
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapModuleEndpoints();

// Fallback to index.html for SPA support
app.MapFallbackToFile("index.html");

if (executionMode != ApplicationExecutionMode.OpenApi)
    await app.ApplyMigrations();

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }