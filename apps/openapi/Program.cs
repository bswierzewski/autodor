using Autodor.API;
using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Infrastructure.Exceptions.Extensions;
using BuildingBlocks.Infrastructure.Identity.Extensions;
using BuildingBlocks.Infrastructure.Modules.Extensions;
using BuildingBlocks.Infrastructure.OpenApi;
using BuildingBlocks.Infrastructure.Wolverine.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Problem details are still registered so build-time OpenAPI generation can describe
// the same structured error contract as the runtime API.
builder.Services.AddProblemDetails(options =>
{
    options.AddDiagnosticInformation();
});

// The document should expose the same authentication requirements as the runtime API,
// so identity services stay registered even though this host never serves real traffic.
builder.Services.AddIdentity(builder.Configuration);

// Reuse the shared OpenAPI enrichments so the generated document includes standardized
// problem details responses and the bearer security scheme.
builder.Services.AddOpenApi(options =>
{
    options.AddProblemDetailsResponses();
    options.AddBearerSecurityScheme();
});

// Module composition is shared with the runtime host to keep the generated document aligned
// with the actual API surface.
builder.Services.ConfigureModules(builder.Configuration, out IModule[] modules);

// Wolverine still needs to discover handlers and endpoint metadata from module assemblies,
// but this OpenAPI host intentionally skips the Postgres-backed runtime wiring.
builder.AddWolverine(modules);

var app = builder.Build();

// Expose the generated document endpoint for the build-time generator.
app.MapOpenApi();

// Map all module-owned minimal APIs so they appear in the generated OpenAPI document.
app.MapModuleEndpoints();

app.Run();