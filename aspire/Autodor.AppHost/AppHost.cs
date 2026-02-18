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
var api = builder.AddProject<Projects.Autodor_API>("api")
    .WaitFor(db)
    .WithReference(db, connectionName: "Default");

// Add Frontend project (Vite + React)
var frontend = builder.AddViteApp("frontend", "../../frontend")
    .WithReference(api);

builder.Build().Run();
