using Autodor.Bootstrap;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Hosting.Extensions;
using BuildingBlocks.Infrastructure.Exceptions.Handlers;
using BuildingBlocks.Infrastructure.Exceptions.Extensions;
using BuildingBlocks.Infrastructure.Identity.Extensions;
using BuildingBlocks.Infrastructure.Modules.Extensions;
using BuildingBlocks.Infrastructure.OpenApi;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using BuildingBlocks.Infrastructure.Serilog.Extensions;
using BuildingBlocks.Infrastructure.Wolverine.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire service defaults
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

// Register authentication and authorization services shared across all modules.
builder.Services.AddIdentity(builder.Configuration);

// Reuse the shared OpenAPI enrichments so the runtime document matches the API's
// standardized error responses and bearer authentication requirements.
builder.Services.AddOpenApi(options =>
{
    options.AddProblemDetailsResponses();
    options.AddBearerSecurityScheme();
});

// Compose modules explicitly so runtime startup and OpenAPI generation share
// the same set of handlers, services, and minimal API endpoints.
IModule[] modules = ModuleCatalog.CreateModules();
builder.Services.RegisterModules(builder.Configuration, modules);

// Wolverine uses the shared Postgres-backed data source in the runtime host so
// handler execution, durability, and transactional messaging are wired together.
var dataSource = builder.Services.AddPostgresDataSource(builder.Configuration, "Default");
builder.AddWolverine(modules, dataSource);

// Configure CORS
builder.Services.AddCors();

var app = builder.Build();

// Run module-specific startup hooks after the container is built.
await app.Services.InitializeModulesAsync();

// Use global exception handler
app.UseExceptionHandler();

// Serve static files and default documents for SPA support
app.UseDefaultFiles();
app.UseStaticFiles();

// Enable authentication and authorization
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

// Map all module-owned minimal APIs into the main application pipeline.
app.MapModuleEndpoints();

// Fallback to index.html for SPA support
app.MapFallbackToFile("index.html");

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }