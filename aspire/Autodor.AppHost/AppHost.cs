DotNetEnv.Env.TraversePath().Load();

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
    .WithReference(db, connectionName: "Default");

// Add Frontend project (Vite + React)
var frontend = builder.AddViteApp("frontend", "../../frontend")
    .WithReference(backend);

// Add Caddy reverse proxy
builder.AddContainer("caddy", "caddy", "alpine")
    .WithBindMount("../../caddy/Caddyfile", "/etc/caddy/Caddyfile", isReadOnly: true)
    .WithVolume("caddy_data", "/data")
    .WithVolume("caddy_config", "/config")
    .WithEnvironment("DOMAIN", Environment.GetEnvironmentVariable("DOMAIN"))
    .WithEnvironment("BACKEND_URL", backend.GetEndpoint("http"))
    .WithEnvironment("FRONTEND_URL", frontend.GetEndpoint("http"))
    // Caddy to jedyna usługa wystawiona "na zewnątrz", więc tylko tutaj definiujemy port 80/443
    .WithEndpoint(port: 80, targetPort: 80, scheme: "http", name: "http")
    .WithEndpoint(port: 443, targetPort: 443, scheme: "https", name: "https")
    .WaitFor(backend)
    .WaitFor(frontend);

builder.Build().Run();
