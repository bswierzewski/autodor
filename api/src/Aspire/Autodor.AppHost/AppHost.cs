// Initializes and runs the Aspire orchestration host that manages all microservices
var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Autodor_Api>("api");

builder.Build().Run();
