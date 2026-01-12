// Initializes and runs the Aspire orchestration host that manages all microservices
var builder = DistributedApplication.CreateBuilder(args);

builder.Build().Run();
