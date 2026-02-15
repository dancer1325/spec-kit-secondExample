// Taskify.AppHost/Program.cs
// T010: Configure Aspire AppHost to orchestrate Postgres, API, and Blazor

var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database with persistent volume
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()  // Persist data across restarts
    .WithPgAdmin();    // Optional: Add pgAdmin for database management

var taskifyDb = postgres.AddDatabase("taskifydb");

// Add API Service with database reference
var apiService = builder.AddProject<Projects.Taskify_ApiService>("apiservice")
    .WithReference(taskifyDb)
    .WaitFor(taskifyDb)
    .WithHttpHealthCheck("/health");

// Add Blazor Server with API reference
builder.AddProject<Projects.Taskify_Web>("webfrontend")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithHttpHealthCheck("/health")
    .WithExternalHttpEndpoints();  // Make accessible from external network

builder.Build().Run();
