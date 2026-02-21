var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "17-alpine")
    //.WithDataVolume() // Persist data between restarts
    .WithPgWeb()      // GUI for managing the database
    ;

var db = postgres
    .AddDatabase("autodor");

// Add API project
var backend = builder.AddProject<Projects.Autodor_API>("backend")
    .WaitFor(db)
    .WithReference(db, connectionName: "Default")
    .WithEndpoint("http", e => e.Port = 7000);

// Add Frontend project (Vite + React)
var frontend = builder.AddViteApp("frontend", "../../frontend")
    .WithReference(backend)
    .WithEndpoint("http", e => e.Port = 3000);

builder.Build().Run();
