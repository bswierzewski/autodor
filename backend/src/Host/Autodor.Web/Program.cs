using BuildingBlocks.Infrastructure.Extensions;
using DotNetEnv;
using Serilog;

// Load environment variables from .env file BEFORE creating builder
// clobberExistingVars: false ensures Docker/CI/CD environment variables take precedence
if (File.Exists(".env"))
    Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with TraceId support
builder.AddSerilog();

// Register core services
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddUserContext();
builder.Services.AddCors();

// ProblemDetails configuration (traceId)
builder.Services.AddProblemDetails(options => 
{
    options.AddCustomConfiguration(builder.Environment);
});

// OpenAPI for Orval client generation
builder.Services.AddOpenApi(options =>
{
    options.AddProblemDetailsSchemas();
});

// Register modules from auto-generated registry
builder.Services.RegisterModules(builder.Configuration, []);

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // OpenAPI endpoint always available at /openapi/v1.json for orval
    app.MapOpenApi();

    // CORS only in Development (production runs in single Docker container)
    app.UseCors(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
}

// Middleware pipeline order matters!
app.UseAuthentication(); // 1. Authentication first
app.UseAuthorization();  // 2. Authorization second

// Configure modules middleware pipeline
// Modules configure their own middleware and endpoints
app.UseModules(builder.Configuration);

// Initialize all modules (run migrations, seed data, etc.)
await app.Services.InitModules();
await app.RunAsync();
