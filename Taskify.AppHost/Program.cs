var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL with database
var postgres = builder.AddPostgres("db")
    .WithDataVolume()
    .AddDatabase("taskifydb");

// Add API Service with reference to Postgres
var api = builder.AddProject<Projects.Taskify_ApiService>("api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithHttpHealthCheck("/health");

// Add Blazor Web with reference to API
builder.AddProject<Projects.Taskify_Web>("blazorapp")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
