// Initializes and runs the Aspire orchestration host that manages all microservices
var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database server
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "17-alpine")
    //.WithDataVolume()
    .WithPgWeb();

var api = builder.AddProject<Projects.Autodor_Api>("api")
    .WithExternalHttpEndpoints(); // Expose API externally

builder.Build().Run();
