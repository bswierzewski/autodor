// Initializes and runs the Aspire orchestration host that manages all microservices
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "17-alpine")
    //.WithDataVolume()
    ;

var api = builder.AddProject<Projects.Autodor_Api>("api")
    .WithExternalHttpEndpoints(); // Expose API externally

builder.Build().Run();
