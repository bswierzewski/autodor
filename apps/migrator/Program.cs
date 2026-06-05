using Autodor.Bootstrap;
using BuildingBlocks.Infrastructure.Modules.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Create the explicit module set shared with the runtime hosts.
var modules = ModuleCatalog.CreateModules();

// Register module services and migration capabilities in the container used by the migrator.
builder.Services.RegisterModules(builder.Configuration, modules);

var app = builder.Build();

// Execute only module-owned migrations and exit without starting the HTTP pipeline.
await app.Services.ApplyModuleMigrationsAsync();