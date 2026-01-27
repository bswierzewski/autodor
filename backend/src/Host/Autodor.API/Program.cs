using Autodor.Modules.Contractors;
using Autodor.Modules.Contractors.Infrastructure.Persistence;
using BuildingBlocks.Infrastructure;
using BuildingBlocks.Infrastructure.Exceptions.Handlers;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Kernel.Abstractions;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with Aspire and Wolverine integration
builder.Host.UseSerilog(logging =>
{
    logging.AddConsole();
    logging.AddOpenTelemetry();
    logging.AddFile();
});

// Add global exception handling
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Dummy implementation of IUserContext for demonstration purposes
// Remove after add IHttpContextAccessor and real user context implementation
builder.Services.AddScoped<IUserContext>(_ => new DummyUserContext());


builder.Host.UseWolverine(opts =>
{
    // Enable FluentValidation middleware for all message handlers
    // This will automatically discover and register validators
    opts.UseFluentValidation();

    // Tell Wolverine that when you have more than one handler for the same
    // message type, they should be executed separately and automatically
    // "stuck" to separate local queues
    opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;

    // Include infrastructure assembly for middleware discovery
    opts.Discovery.IncludeAssembly(typeof(InfrastructureAssembly).Assembly);

    // Add all of the message handlers, etc. from the modules
    // This also discovers middleware from these assemblies
    opts.Discovery.IncludeAssembly(typeof(ContractorsModule).Assembly);
    //opts.Discovery.IncludeAssembly(typeof(OrdersModule).Assembly);
    //opts.Discovery.IncludeAssembly(typeof(InvoicingModule).Assembly);
});

// Add services to the container.
builder.Services.AddWolverineHttp();

// Configure CORS
builder.Services.AddCors();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddProblemDetailsResponses();
});

var app = builder.Build();

// Use global exception handler
app.UseExceptionHandler();

//app.MapDefaultEndpoints();

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
}

app.MapWolverineEndpoints();

await app.MigrateDatabaseAsync<ContractorsDbContext>();
//await app.MigrateDatabaseAsync<OrdersDbContext>();
//await app.MigrateDatabaseAsync<InvoicingModule>();

app.Run();

// Inline dummy implementation of IUserContext for testing
class DummyUserContext : IUserContext
{
    // Statyczny Guid dla testów - w prawdziwej aplikacji to byłby UserId z ClaimsPrincipal
    public Guid Id => Guid.Parse("00000000-0000-0000-0000-000000000001");

    public IEnumerable<string> Roles => new[] { "User", "Admin" };

    public bool IsInRole(string role)
    {
        return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}

// Make Program class accessible to integration tests
public partial class Program { }