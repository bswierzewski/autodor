var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database container
var postgresPassword = builder.AddParameter("postgres-password", "autodor_local_password", secret: true);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "18-alpine")
    .WithPassword(postgresPassword)
    .WithDataVolume() // Persist data between restarts
    .WithPgWeb(); // GUI for managing the database

// Add database
var database = postgres.AddDatabase("db");

// Add API project
var api = builder.AddProject<Projects.Autodor_API>("api")
    .WithReference(database, "Default")
    .WaitFor(database)
    .WithHttpHealthCheck("/health")
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar";
        url.Url = "/scalar";
    });

// Add Frontend project (Vite + React)
var web = builder.AddViteApp("web", "../web")
    .WithHttpHealthCheck("/");

builder.AddYarp("gateway")
    .WithHttpsEndpoint()
    .WithHttpsDeveloperCertificate()
    .WaitFor(api)
    .WaitFor(web)
    .WithHttpHealthCheck("/")
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute("/api/{**catch-all}", api);
        yarp.AddRoute("/{**catch-all}", web);
    });

builder.Build().Run();