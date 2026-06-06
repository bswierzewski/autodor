using Autodor.Bootstrap;
using Autodor.Migrator.Services;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Infrastructure.Modules.Extensions;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add time provider
builder.Services.TryAddSingleton(TimeProvider.System);

// Register a migrator implementation of ICurrentUser for scenarios where no user context is available (e.g., migrations).
// This ensures that services like AuditableEntityInterceptor can be instantiated even when running outside of an HTTP context.
builder.Services.TryAddScoped<ICurrentUser, CurrentUser>();

// Create the explicit module set shared with the runtime hosts.
var modules = ModuleCatalog.CreateModules();

// Register the shared Postgres data source required by module DbContexts during migration execution.
builder.Services.AddPostgresDataSource(builder.Configuration, "Default");

// Register module services and migration capabilities in the container used by the migrator.
builder.Services.RegisterModules(builder.Configuration, modules);

var app = builder.Build();

// Execute only module-owned migrations and exit without starting the HTTP pipeline.
await app.Services.ApplyModuleMigrationsAsync();
