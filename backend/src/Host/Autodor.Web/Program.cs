using DotNetEnv;
using Shared.Abstractions.Modules;
using Shared.Infrastructure.Exceptions;
using Shared.Infrastructure.Modules;
using Shared.Users.Infrastructure.Extensions.Supabase;

// Load environment variables from .env file BEFORE creating builder
// clobberExistingVars: false ensures Docker/CI/CD environment variables take precedence
if (File.Exists(".env"))
    Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Register core services
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpContextAccessor();

// Exception handling
builder.Services.AddExceptionHandler<CustomExceptionHandler>();
builder.Services.AddProblemDetails();

// OpenAPI for Orval client generation
builder.Services.AddEndpointsApiExplorer(); // Exposes Minimal API endpoints to OpenAPI
builder.Services.AddOpenApi();              // Generates OpenAPI document

// Register modules from auto-generated registry
builder.Services.RegisterModules(builder.Configuration);

builder.Services.AddAuthentication()
    .AddSupabaseJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // CORS only in Development (production runs in single Docker container)
    app.UseCors(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
}

// Exception handling
app.UseExceptionHandler(options => { });

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
