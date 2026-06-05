using Autodor.Bootstrap;
using BuildingBlocks.Infrastructure.Modules.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Reuse the shared module catalog so the migrator targets the same schemas as the runtime hosts.
builder.Services.RegisterModules(builder.Configuration, ModuleCatalog.CreateModules());

var app = builder.Build();

// Execute only module-owned migrations and exit without starting the HTTP pipeline.
await app.Services.ApplyModuleMigrationsAsync();