using Microsoft.EntityFrameworkCore;
using Taskify.ApiService.Data;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (Aspire)
builder.AddServiceDefaults();

// Add DbContext with Postgres
builder.Services.AddDbContext<TaskifyDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("taskifydb");
    options.UseNpgsql(connectionString);
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Taskify API",
        Version = "v1",
        Description = "REST API for Taskify task management application"
    });
});

// Add output caching for health checks
builder.Services.AddOutputCache();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Map default endpoints (health checks)
app.MapDefaultEndpoints();

// Apply migrations on startup (development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskifyDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.Run();
