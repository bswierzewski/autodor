using DotNetEnv;
using Shared.Abstractions.Modules;
using Shared.Infrastructure.Exceptions;
using Shared.Infrastructure.Logging;
using Shared.Infrastructure.OpenApi;
using Shared.Users.Infrastructure.Extensions.Supabase;

// Load environment variables from .env file BEFORE creating builder
// clobberExistingVars: false ensures Docker/CI/CD environment variables take precedence
if (File.Exists(".env"))
    Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with TraceId support
builder.AddSerilog("Autodor.Backend");

// Register core services
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpContextAccessor();

// Exception handling
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddProblemDetails();

// OpenAPI for Orval client generation
builder.Services.AddOpenApi(options =>
{
    options.AddSchemaTransformer<ApiProblemDetailsSchemaTransformer>();
});

// CORS configuration
builder.Services.AddCors();

// Register modules from auto-generated registry
builder.Services.RegisterModules(builder.Configuration);

builder.Services.AddAuthentication()
    .AddSupabaseJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

// OpenAPI endpoint always available at /openapi/v1.json for orval
app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
    // CORS only in Development (production runs in single Docker container)
    app.UseCors(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
}

// Exception handling
app.UseExceptionHandler(options => { });

// Request logging with TraceId (MUST be after UseExceptionHandler)
app.UseSerilogRequestLogging();

// Middleware pipeline order matters!
app.UseAuthentication(); // 1. Authentication first
app.UseAuthorization();  // 2. Authorization second

// Configure modules middleware pipeline
// Modules configure their own middleware and endpoints
app.UseModules(builder.Configuration);

// Initialize all modules (run migrations, seed data, etc.)
await app.Services.InitModules();

await app.RunAsync();

// Make the Program class accessible for integration tests
// [GenerateModuleRegistry] triggers source generator to create ModuleRegistry class
[GenerateModuleRegistry]
public partial class Program { }
