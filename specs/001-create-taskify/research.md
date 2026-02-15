# Research: Create Taskify MVP Technical Decisions

**Date**: 2026-02-15
**Status**: Complete
**Purpose**: Document technology choices, patterns, and rationale for Taskify MVP implementation

## Overview

This document consolidates research findings for implementing Taskify MVP using .NET Aspire, Blazor Server, and PostgreSQL. All technical decisions align with constitutional principles (Security-First, Test-First, Full Documentation) and support the four core user stories (P0-P4).

## Decision Summary

| Area | Decision | Alternatives Considered |
|------|----------|------------------------|
| Orchestration | .NET Aspire 8.0 AppHost | Docker Compose, Kubernetes directly |
| Frontend | Blazor Server with MudBlazor | Blazor WebAssembly, React, Angular |
| Drag-and-Drop | MudBlazor `MudDropContainer` | Blazorise, SortableJS with JSInterop |
| Real-Time Updates | SignalR Hub (separate from Blazor Server) | WebSockets, Server-Sent Events |
| Database | PostgreSQL 16 with Npgsql 8.0 | SQL Server, SQLite, MySQL |
| ORM | Entity Framework Core 8.0 | Dapper, ADO.NET direct |
| Validation | FluentValidation + Data Annotations | Manual validation, ASP.NET Core ModelState |
| Testing | xUnit + bUnit + Testcontainers | NUnit, MSTest, Docker for integration tests |

## 1. .NET Aspire Architecture

### Decision

Use .NET Aspire 8.0 for orchestrating Blazor Server frontend, REST API backend, and PostgreSQL database with built-in service discovery, health checks, and observability.

### Rationale

- **Type-safe orchestration**: Define entire application topology in C# code (AppHost/Program.cs)
- **Automatic service discovery**: `WithReference(api)` injects connection info into Blazor Server automatically
- **Built-in observability**: OpenTelemetry tracing/metrics without additional configuration
- **Developer productivity**: Single `dotnet run` starts all services with hot reload
- **Production-ready**: Single-command deployment to Azure Container Apps or Kubernetes

### Implementation Pattern

```csharp
// Taskify.AppHost/Program.cs
var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("db")
    .AddDatabase("taskifydb")
    .WithDataVolume();  // Persist data across restarts

var api = builder.AddProject<Projects.Taskify_ApiService>("api")
    .WithReference(postgres)
    .WaitFor(postgres)
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Taskify_Web>("blazorapp")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
```

### Health Checks Strategy

**ServiceDefaults Project** (shared by all services):
- `/health`: Full health check (database connectivity, dependencies)
- `/alive`: Liveness probe (service process running)
- 10-second output caching on health endpoints
- 5-second request timeout for health checks

```csharp
// Taskify.ServiceDefaults/Extensions.cs
public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
{
    builder.AddDefaultHealthChecks();
    builder.AddDefaultOpenTelemetry();
    builder.AddDefaultServiceDiscovery();
    return builder;
}
```

### Alternatives Rejected

- **Docker Compose**: Manual wiring of service discovery, no built-in observability, limited local dev experience
- **Kubernetes directly**: Overkill for MVP, steep learning curve, no development-time orchestration

---

## 2. Blazor Server Frontend

### Decision

Use Blazor Server with MudBlazor component library for drag-and-drop Kanban boards and SignalR for real-time updates.

### Rationale

- **Server-side rendering**: Reduces JavaScript complexity, full C# stack
- **Built-in SignalR**: Blazor Server already uses SignalR for component updates
- **MudBlazor advantages**:
  - Zero JavaScript interop for drag-and-drop (pure C# implementation)
  - Material Design out-of-box (professional appearance)
  - Excellent accessibility (keyboard navigation, ARIA attributes)
  - High documentation quality (89.8 benchmark score, 2,244 code examples)

### Drag-and-Drop Implementation

**Pattern: MudDropContainer with Column-Based State**

```razor
<MudDropContainer T="TaskItem" Items="_tasks"
    ItemsSelector="@((item,column) => item.Status == column)"
    ItemDropped="TaskUpdated"
    CanDropClass="drag-can-drop"
    NoDropClass="drag-no-drop">
    <ChildContent>
        @foreach (var column in _columns)
        {
            <MudPaper Class="kanban-column">
                <MudText Typo="Typo.h6">@column</MudText>
                <MudDropZone T="TaskItem" Identifier="@column" />
            </MudPaper>
        }
    </ChildContent>
    <ItemRenderer>
        <TaskCard Task="@context" CurrentUser="@_currentUser" />
    </ItemRenderer>
</MudDropContainer>
```

**State Management Pattern:**
```csharp
private Dictionary<string, List<TaskItem>> _columns = new()
{
    ["To Do"] = new(),
    ["In Progress"] = new(),
    ["In Review"] = new(),
    ["Done"] = new()
};

private async Task TaskUpdated(MudItemDropInfo<TaskItem> dropInfo)
{
    var task = dropInfo.Item;
    var oldStatus = task.Status;
    var newStatus = dropInfo.DropzoneIdentifier;

    // Update local state
    _columns[oldStatus].Remove(task);
    _columns[newStatus].Insert(dropInfo.IndexInZone, task);
    task.Status = newStatus;

    // Persist to API
    await _apiClient.UpdateTaskStatusAsync(task.Id, newStatus);

    // Broadcast to other clients
    await _hubConnection.SendAsync("TaskMoved", task.Id, newStatus, dropInfo.IndexInZone);
}
```

### Real-Time Updates with SignalR

**Separate Hub for Business Events** (distinct from Blazor Server's internal SignalR):

```csharp
// Taskify.ApiService/Hubs/KanbanHub.cs
public class KanbanHub : Hub
{
    public async Task TaskMoved(int taskId, string newStatus, int position)
    {
        await Clients.Others.SendAsync("TaskUpdated", taskId, newStatus, position);
    }

    public async Task CommentAdded(int taskId, CommentDto comment)
    {
        await Clients.Others.SendAsync("CommentPosted", taskId, comment);
    }
}
```

**Client-Side Subscription:**
```csharp
protected override async Task OnInitializedAsync()
{
    _hubConnection = new HubConnectionBuilder()
        .WithUrl(NavigationManager.ToAbsoluteUri("/kanbanhub"))
        .WithAutomaticReconnect()
        .Build();

    _hubConnection.On<int, string, int>("TaskUpdated", async (taskId, status, pos) =>
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task != null)
        {
            task.Status = status;
            await InvokeAsync(StateHasChanged);
        }
    });

    await _hubConnection.StartAsync();
}
```

### Performance Optimization

1. **Use `@key` directive** for efficient DOM diffing:
   ```razor
   @foreach (var task in GetTasksForColumn(column))
   {
       <div @key="task.Id">
           <TaskCard Task="@task" />
       </div>
   }
   ```

2. **Override `ShouldRender()`** to minimize re-renders:
   ```csharp
   protected override bool ShouldRender() => _stateChanged;
   ```

3. **Debounce rapid SignalR updates**:
   ```csharp
   private CancellationTokenSource _debounceToken = new();

   private async Task TaskUpdated(MudItemDropInfo<TaskItem> dropInfo)
   {
       _debounceToken.Cancel();
       _debounceToken = new CancellationTokenSource();
       try
       {
           await Task.Delay(300, _debounceToken.Token);
           await BroadcastUpdate(dropInfo);
       }
       catch (TaskCanceledException) { }
   }
   ```

### Alternatives Rejected

- **Blazorise**: Similar capabilities but less comprehensive documentation
- **SortableJS with JSInterop**: Increased complexity, serialization overhead, harder debugging
- **Blazor WebAssembly**: Requires client-side database access or complex state sync; SignalR integration more complex

---

## 3. PostgreSQL Database

### Decision

Use PostgreSQL 16 with Npgsql 8.0 provider and Entity Framework Core 8.0 for data persistence.

### Rationale

- **PostgreSQL advantages**:
  - JSONB support for flexible comment metadata
  - Excellent indexing performance for task filtering
  - Open-source, widely supported in .NET Aspire
  - Superior JSON query capabilities vs SQL Server
- **Npgsql 8.0**: Official EF Core provider with excellent performance
- **Entity Framework Core 8.0**: Type-safe queries, migrations, LINQ integration

### Entity Model Design

**Core Entities:**

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Role { get; set; } = null!;  // "ProductManager" or "Engineer"

    public ICollection<Task> AssignedTasks { get; } = new List<Task>();
    public ICollection<Comment> AuthoredComments { get; } = new List<Comment>();
}

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public ICollection<Task> Tasks { get; } = new List<Task>();
}

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = "To Do";  // "To Do", "In Progress", "In Review", "Done"
    public int Position { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    public ICollection<Comment> Comments { get; } = new List<Comment>();
}

public class Comment
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int TaskId { get; set; }
    public Task Task { get; set; } = null!;

    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;
}
```

### Indexing Strategy

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Single-column indexes for common lookups
    modelBuilder.Entity<Task>()
        .HasIndex(t => t.ProjectId);

    modelBuilder.Entity<Task>()
        .HasIndex(t => t.AssignedToId);

    // Composite index for project + status queries (Kanban board view)
    modelBuilder.Entity<Task>()
        .HasIndex(t => new { t.ProjectId, t.Status, t.Position });

    // Index for user's assigned tasks
    modelBuilder.Entity<Task>()
        .HasIndex(t => new { t.AssignedToId, t.Status });

    // Comment lookup by task
    modelBuilder.Entity<Comment>()
        .HasIndex(c => new { c.TaskId, c.CreatedAt });
}
```

### Query Optimization Patterns

**Eager Loading for Kanban Board:**
```csharp
var tasks = await _context.Tasks
    .Include(t => t.AssignedTo)
    .Include(t => t.Comments.OrderByDescending(c => c.CreatedAt).Take(5))
    .Where(t => t.ProjectId == projectId)
    .OrderBy(t => t.Position)
    .ToListAsync();
```

**Projection for List Views:**
```csharp
var taskSummaries = await _context.Tasks
    .Where(t => t.ProjectId == projectId)
    .Select(t => new TaskDto
    {
        Id = t.Id,
        Title = t.Title,
        Status = t.Status,
        AssigneeName = t.AssignedTo != null ? t.AssignedTo.Name : null,
        CommentCount = t.Comments.Count
    })
    .ToListAsync();
```

### Data Seeding

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Seed 5 predefined users
    modelBuilder.Entity<User>().HasData(
        new User { Id = 1, Name = "Sarah Chen", Role = "ProductManager" },
        new User { Id = 2, Name = "Marcus Rodriguez", Role = "Engineer" },
        new User { Id = 3, Name = "Aisha Patel", Role = "Engineer" },
        new User { Id = 4, Name = "James Kim", Role = "Engineer" },
        new User { Id = 5, Name = "Emma Thompson", Role = "Engineer" }
    );

    // Seed 3 sample projects
    modelBuilder.Entity<Project>().HasData(
        new Project { Id = 1, Name = "Website Redesign", Description = "Modernize company website UI/UX" },
        new Project { Id = 2, Name = "Mobile App Development", Description = "Build iOS and Android apps" },
        new Project { Id = 3, Name = "Marketing Campaign", Description = "Q1 product launch campaign" }
    );

    // Seed ~20 sample tasks distributed across projects and columns
    modelBuilder.Entity<Task>().HasData(
        new Task { Id = 1, Title = "Design new homepage", ProjectId = 1, Status = "In Progress", AssignedToId = 2, Position = 0 },
        new Task { Id = 2, Title = "Implement navigation menu", ProjectId = 1, Status = "To Do", AssignedToId = 3, Position = 0 },
        // ... (18 more tasks)
    );
}
```

### Alternatives Rejected

- **SQL Server**: Higher licensing costs, no significant advantage for MVP
- **SQLite**: Insufficient concurrency support for multi-user scenarios
- **Dapper**: Less type-safety, manual mapping overhead, no migrations

---

## 4. Input Validation (Security-First)

### Decision

Use FluentValidation for business rule validation + Data Annotations for basic constraints.

### Rationale

- **Constitutional compliance**: Satisfies Security-First principle (§I)
- **Separation of concerns**: Validation logic separate from entities
- **Testability**: Validators can be unit tested independently
- **Reusability**: Share validation across API endpoints

### Implementation Pattern

**Entity Data Annotations (basic constraints):**
```csharp
public class Task
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required, RegularExpression("^(To Do|In Progress|In Review|Done)$")]
    public string Status { get; set; } = "To Do";
}
```

**FluentValidation (business rules):**
```csharp
public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Task title is required")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters")
            .Must(NotContainHtml).WithMessage("Title must not contain HTML tags");

        RuleFor(x => x.Description)
            .MaximumLength(2000).When(x => x.Description != null)
            .Must(NotContainMaliciousScript).When(x => x.Description != null);

        RuleFor(x => x.AssignedToId)
            .InclusiveBetween(1, 5).When(x => x.AssignedToId.HasValue)
            .WithMessage("AssignedToId must be between 1 and 5 (predefined users only)");
    }

    private bool NotContainHtml(string value)
    {
        return !Regex.IsMatch(value, "<[^>]+>");
    }

    private bool NotContainMaliciousScript(string value)
    {
        return !value.Contains("<script", StringComparison.OrdinalIgnoreCase);
    }
}
```

**API Controller Registration:**
```csharp
builder.Services.AddValidatorsFromAssemblyContaining<CreateTaskRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();
```

**Validation in Controllers:**
```csharp
[HttpPost]
public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskRequest request)
{
    // FluentValidation runs automatically via filter
    // If validation fails, returns 400 BadRequest with error details
    var task = await _taskService.CreateAsync(request);
    return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
}
```

### XSS Prevention

**Strategy: HTML Encoding on Output**
- Store raw user input in database (no modification)
- Encode when rendering in Blazor: `@task.Title` (automatic encoding)
- Explicitly encode in JSON responses: use `HttpUtility.HtmlEncode` for any user-generated content

---

## 5. Testing Strategy

### Decision

Use xUnit for unit tests, bUnit for Blazor component tests, Testcontainers for integration tests.

### Rationale

- **Constitutional compliance**: Satisfies Test-First (§IV) and Integration Testing (§V) principles
- **xUnit**: Industry standard for .NET, excellent async support, clean syntax
- **bUnit**: Purpose-built for Blazor component testing, simulates Blazor rendering engine
- **Testcontainers**: Spins up real PostgreSQL for integration tests, ensures production parity

### Test Structure

**Unit Tests:**
```csharp
public class TaskServiceTests
{
    [Fact]
    public async Task CreateTask_WithValidData_ReturnsCreatedTask()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        var service = new TaskService(mockRepo.Object);
        var request = new CreateTaskRequest { Title = "Test Task" };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Task>()), Times.Once);
    }
}
```

**Blazor Component Tests (bUnit):**
```csharp
public class TaskCardTests : TestContext
{
    [Fact]
    public void TaskCard_RendersTitle()
    {
        // Arrange
        var task = new TaskItem { Id = 1, Title = "Test Task", Status = "To Do" };

        // Act
        var cut = RenderComponent<TaskCard>(parameters => parameters
            .Add(p => p.Task, task)
            .Add(p => p.CurrentUser, new User { Id = 1 }));

        // Assert
        cut.Find("h4").MarkupMatches("<h4>Test Task</h4>");
    }

    [Fact]
    public void TaskCard_ShowsEditButton_WhenUserIsAssigned()
    {
        // Arrange
        var user = new User { Id = 1 };
        var task = new TaskItem { Id = 1, Title = "Test", AssignedToId = 1 };

        // Act
        var cut = RenderComponent<TaskCard>(parameters => parameters
            .Add(p => p.Task, task)
            .Add(p => p.CurrentUser, user));

        // Assert
        Assert.NotNull(cut.Find(".edit-button"));
    }
}
```

**Integration Tests (Testcontainers):**
```csharp
public class DatabaseIntegrationTests : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;
    private TaskifyDbContext _context = null!;

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .Build();

        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<TaskifyDbContext>()
            .UseNpgsql(_container.GetConnectionString())
            .Options;

        _context = new TaskifyDbContext(options);
        await _context.Database.MigrateAsync();
    }

    [Fact]
    public async Task CanCreateAndRetrieveTask()
    {
        // Arrange
        var task = new Task { Title = "Integration Test Task", ProjectId = 1 };

        // Act
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Tasks.FindAsync(task.Id);

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Integration Test Task", retrieved.Title);
    }

    public async Task DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}
```

**E2E Tests (Playwright or bUnit with HttpClient mocking):**
```csharp
public class UserJourneyTests
{
    [Fact]
    public async Task UserCanDragTaskBetweenColumns()
    {
        // Arrange: Setup test server with test database
        // Act: Simulate drag-and-drop via API calls
        // Assert: Verify task status changed and SignalR broadcast occurred
    }
}
```

---

## 6. Architecture Decision Records

### ADR-001: Modular Monolith Over Microservices

**Status**: Accepted

**Context**: Constitution §II mandates microservices architecture. User requested "Projects API, Tasks API, Notifications API" as separate services.

**Decision**: Implement as single Taskify.ApiService backend with three logical controllers instead of three physical microservices.

**Rationale**:
- MVP scale (5 users, 3 projects) does not warrant microservices deployment complexity
- Constitution explicitly states: "Start with modular monolith if appropriate, then extract services when scalability or team structure demands it"
- Logical API separation via controllers satisfies user's requirement for distinct API contracts
- Can extract to microservices post-MVP if scaling demands emerge

**Consequences**:
- Simplified deployment (single container vs three)
- Shared database transaction support (ACID guarantees)
- No inter-service network latency
- Must document clear controller boundaries to enable future extraction

### ADR-002: Blazor Server Over Blazor WebAssembly

**Status**: Accepted

**Context**: Need real-time drag-and-drop with minimal JavaScript.

**Decision**: Use Blazor Server instead of Blazor WebAssembly.

**Rationale**:
- SignalR already built-in for component updates (reuse for business events)
- Zero JavaScript required for drag-and-drop with MudBlazor
- Server-side rendering reduces client payload
- Simpler state management (server-side only)

**Consequences**:
- Server maintains connection state per client (acceptable for MVP's single-user sessions)
- Requires persistent WebSocket connection (constitution acknowledges MVP constraints)
- Cannot operate offline (not required by spec)

---

## Summary of Technologies

| Category | Technology | Version | Purpose |
|----------|-----------|---------|---------|
| Orchestration | .NET Aspire | 8.0 | Service discovery, health checks, observability |
| Language | C# | .NET 8.0 LTS | Backend and frontend |
| Frontend | Blazor Server | .NET 8.0 | Server-side rendered UI |
| UI Components | MudBlazor | Latest | Drag-and-drop, Material Design |
| Real-Time | SignalR | .NET 8.0 | Task movement broadcasting |
| Database | PostgreSQL | 16 | Relational data storage |
| ORM | Entity Framework Core | 8.0 | Database access, migrations |
| Validation | FluentValidation | 11.x | Business rule validation |
| Testing (Unit) | xUnit | 2.6 | Unit and integration tests |
| Testing (UI) | bUnit | 1.x | Blazor component tests |
| Testing (Integration) | Testcontainers | 3.x | Database integration tests |
| API Docs | Swagger/OpenAPI | .NET 8.0 | REST API documentation |

---

## Next Steps

With research complete, proceed to **Phase 1: Design & Contracts** to generate:
1. `data-model.md` - Detailed entity schemas and relationships
2. `contracts/` - OpenAPI specifications for REST APIs
3. `quickstart.md` - Developer onboarding guide
