var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database container
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "18-alpine")
    //.WithDataVolume() // Persist data between restarts
    .WithPgWeb(); // GUI for managing the database

// Add database
var database = postgres.AddDatabase("db");

// Add API project
var api = builder.AddProject<Projects.Autodor_API>("api")
    .WithReference(database, "Default")
    .WaitFor(database)
    .WithHttpHealthCheck("/health");

// Add Frontend project (Vite + React)
var app = builder.AddViteApp("app", "../../frontend")
    .WithHttpHealthCheck("/");

builder.AddYarp("gateway")
    .WithHttpsEndpoint()
    .WithHttpsDeveloperCertificate()
    .WaitFor(api)
    .WaitFor(app)
    .WithHttpHealthCheck("/")
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute("/api/{**catch-all}", api);
        yarp.AddRoute("/{**catch-all}", app);
    });

builder.Build().Run();