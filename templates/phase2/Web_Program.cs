// Taskify.Web/Program.cs
// T022: Configure Program.cs for Blazor Server and MudBlazor

using MudBlazor.Services;
using Taskify.Web.Components;
using Taskify.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (health checks, telemetry, service discovery)
builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add application services
builder.Services.AddScoped<UserContextService>();

// Configure HttpClient for API communication with service discovery
builder.Services.AddHttpClient<TaskifyApiClient>(client =>
{
    // Aspire service discovery will resolve "apiservice" to the actual URL
    client.BaseAddress = new Uri("https+http://apiservice");
});

// Add SignalR client services
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map health check endpoints
app.MapDefaultEndpoints();

app.Run();
