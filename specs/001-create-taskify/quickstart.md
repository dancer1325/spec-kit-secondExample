# Taskify MVP - Developer Quickstart Guide

**Date**: 2026-02-15
**Version**: 1.0.0
**Estimated Setup Time**: 15-20 minutes

## Overview

This guide will help you set up Taskify MVP on your local development machine. By the end, you'll have:
- .NET Aspire AppHost orchestrating all services
- Blazor Server frontend running with MudBlazor components
- REST API backend with three controllers
- PostgreSQL database with sample data
- SignalR hub for real-time updates

## Prerequisites

### Required Software

| Software | Minimum Version | Purpose | Installation Link |
|----------|----------------|---------|------------------|
| .NET SDK | 8.0 | Runtime and tooling | https://dotnet.microsoft.com/download/dotnet/8.0 |
| .NET Aspire Workload | 8.0 | Orchestration and service discovery | `dotnet workload install aspire` |
| Docker Desktop | Latest | PostgreSQL container | https://www.docker.com/products/docker-desktop/ |
| IDE | Any | Code editing | VS Code, Visual Studio 2022, or Rider |
| Git | 2.x | Source control | https://git-scm.com/downloads |

### Optional Tools

- **Azure Data Studio** or **pgAdmin**: PostgreSQL GUI for database inspection
- **Bruno** or **Postman**: API testing (though Swagger UI is included)
- **dotnet-ef CLI**: Manual database migrations (`dotnet tool install --global dotnet-ef`)

### System Requirements

- **OS**: Windows 10+, macOS 12+, or Linux (Ubuntu 20.04+)
- **RAM**: 8GB minimum, 16GB recommended
- **Disk Space**: 5GB free space
- **Ports**: 5000-5002, 5432 (PostgreSQL), 7000-7002 must be available

## Installation Steps

### 1. Install .NET 8.0 SDK

```bash
# Verify installation
dotnet --version
# Should output: 8.0.x

# Check installed SDKs
dotnet --list-sdks
```

### 2. Install .NET Aspire Workload

```bash
# Install Aspire workload
dotnet workload install aspire

# Verify installation
dotnet workload list
# Should show: aspire
```

### 3. Install Docker Desktop

1. Download from https://www.docker.com/products/docker-desktop/
2. Install and start Docker Desktop
3. Verify Docker is running:

```bash
docker --version
# Should output: Docker version 24.x.x or higher

docker ps
# Should list running containers (may be empty)
```

### 4. Clone Repository

```bash
git clone <repository-url>
cd taskify
git checkout 001-create-taskify
```

## Project Structure

```text
Taskify/
├── Taskify.sln                      # Solution file
├── Taskify.AppHost/                 # .NET Aspire orchestration
│   └── Program.cs
├── Taskify.ServiceDefaults/         # Shared service configuration
│   └── Extensions.cs
├── Taskify.Web/                     # Blazor Server frontend
│   ├── Components/
│   ├── Services/
│   └── Program.cs
├── Taskify.ApiService/              # REST API backend
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   └── Program.cs
└── Tests/
    ├── Taskify.Web.Tests/
    ├── Taskify.ApiService.Tests/
    └── Taskify.E2E.Tests/
```

## Running the Application

### Quick Start (Recommended)

**Single Command to Start Everything**:

```bash
cd Taskify.AppHost
dotnet run
```

This starts:
1. PostgreSQL container on port 5432
2. API Service on https://localhost:7001
3. Blazor Server app on https://localhost:7000
4. Aspire Dashboard on https://localhost:18888

**Access Points**:
- **Taskify UI**: https://localhost:7000
- **Aspire Dashboard**: https://localhost:18888 (monitoring, logs, traces)
- **API Swagger**: https://localhost:7001/swagger

### Step-by-Step Start (Manual)

If you prefer to start services individually:

#### 1. Start PostgreSQL

```bash
docker run -d \
  --name taskify-postgres \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_DB=taskifydb \
  -p 5432:5432 \
  postgres:16
```

#### 2. Apply Database Migrations

```bash
cd Taskify.ApiService
dotnet ef database update
```

#### 3. Start API Service

```bash
cd Taskify.ApiService
dotnet run
# API available at https://localhost:7001
```

#### 4. Start Blazor Server App

```bash
# In a new terminal
cd Taskify.Web
dotnet run
# UI available at https://localhost:7000
```

## First-Time Setup

### Database Initialization

The database is automatically seeded with:
- **5 users**: 1 Product Manager (Sarah Chen) + 4 Engineers
- **3 projects**: Website Redesign, Mobile App Development, Marketing Campaign
- **20 tasks**: Distributed across projects and status columns

**Verification**:

```bash
# Connect to database
docker exec -it taskify-postgres psql -U postgres -d taskifydb

# Check seeded data
SELECT * FROM "Users";
SELECT COUNT(*) FROM "Tasks";
\q
```

### Configuration Files

**Taskify.ApiService/appsettings.Development.json**:
```json
{
  "ConnectionStrings": {
    "taskifydb": "Host=localhost;Port=5432;Database=taskifydb;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

**Taskify.Web/appsettings.Development.json**:
```json
{
  "Services": {
    "TaskifyApi": {
      "Https": "https://localhost:7001"
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore.SignalR": "Debug"
    }
  }
}
```

## Using the Application

### 1. User Selection (P0 - User Story 0)

1. Navigate to https://localhost:7000
2. You'll see the user selection screen with 5 users:
   - Sarah Chen (Product Manager)
   - Marcus Rodriguez (Engineer)
   - Aisha Patel (Engineer)
   - James Kim (Engineer)
   - Emma Thompson (Engineer)
3. Click any user to enter the application

### 2. Project List (P0)

After selecting a user, you'll see:
- List of 3 sample projects
- Each project shows name, description, and task count
- Click a project to view its Kanban board

### 3. Kanban Board (P1 - User Story 1)

- **Four columns**: To Do, In Progress, In Review, Done
- **Drag-and-drop**: Drag tasks between columns to change status
- **Visual highlighting**: Tasks assigned to current user shown in accent color
- **Real-time updates**: Changes broadcast via SignalR to other browser tabs

### 4. Task Management (P2 - User Story 2)

- **Create task**: Click "+" button, enter title/description, select assignee
- **View task**: Click task card to open detail modal
- **Edit task**: Click description in modal to edit inline
- **Reassign task**: Use assignee dropdown in modal to change assignment

### 5. Comments (P4 - User Story 4)

- **Add comment**: In task modal, type in comment input at bottom and click "Submit"
- **Edit comment**: Click "Edit" button on your own comments (inline editing)
- **Delete comment**: Click "Delete" button on your own comments (confirmation dialog appears)
- **Permissions**: You can only edit/delete your own comments

## Development Workflow

### Hot Reload

Blazor Server supports hot reload:

```bash
# In Taskify.Web directory
dotnet watch
```

Changes to Razor files trigger automatic browser refresh.

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
cd Tests/Taskify.ApiService.Tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Database Migrations

**Create a new migration**:
```bash
cd Taskify.ApiService
dotnet ef migrations add <MigrationName>
```

**Apply migrations**:
```bash
dotnet ef database update
```

**Rollback migration**:
```bash
dotnet ef database update <PreviousMigrationName>
```

### API Testing

**Swagger UI**: https://localhost:7001/swagger

**Example API Calls with curl**:

```bash
# List all projects
curl https://localhost:7001/v1/projects

# Get project with tasks
curl https://localhost:7001/v1/projects/1

# Create a new task
curl -X POST https://localhost:7001/v1/projects/1/tasks \
  -H "Content-Type: application/json" \
  -d '{"title":"New Task","assignedToId":2}'

# Update task status (drag-and-drop)
curl -X PATCH https://localhost:7001/v1/tasks/1/status \
  -H "Content-Type: application/json" \
  -d '{"status":"In Progress","position":0}'

# Add a comment
curl -X POST https://localhost:7001/v1/tasks/1/comments \
  -H "Content-Type: application/json" \
  -d '{"content":"Great work!","authorId":1}'
```

## Troubleshooting

### Common Issues

**1. Port Already in Use**

```bash
# Find process using port 5432 (PostgreSQL)
lsof -i :5432
# or on Windows:
netstat -ano | findstr :5432

# Kill the process
kill -9 <PID>
```

**2. Database Connection Failure**

```bash
# Check if PostgreSQL container is running
docker ps | grep taskify-postgres

# View container logs
docker logs taskify-postgres

# Restart container
docker restart taskify-postgres
```

**3. Aspire Dashboard Not Accessible**

```bash
# Check Aspire AppHost logs
cd Taskify.AppHost
dotnet run --verbosity detailed
```

**4. Hot Reload Not Working**

```bash
# Clear browser cache
# Restart dotnet watch
dotnet watch --no-hot-reload
```

### Reset Database

```bash
# Stop API service
# Drop and recreate database
docker exec -it taskify-postgres psql -U postgres -c "DROP DATABASE IF EXISTS taskifydb;"
docker exec -it taskify-postgres psql -U postgres -c "CREATE DATABASE taskifydb;"

# Re-apply migrations
cd Taskify.ApiService
dotnet ef database update
```

### Clean Build

```bash
# Remove bin/obj folders and rebuild
dotnet clean
dotnet build
```

## IDE Setup

### Visual Studio 2022

1. Open `Taskify.sln`
2. Set `Taskify.AppHost` as startup project
3. Press F5 to start with debugging
4. Aspire Dashboard opens automatically in browser

### Visual Studio Code

1. Install extensions:
   - C# Dev Kit
   - .NET Aspire (preview)
2. Open workspace folder
3. F5 to debug, select "Taskify.AppHost"

### JetBrains Rider

1. Open `Taskify.sln`
2. Right-click `Taskify.AppHost` → "Run"
3. Aspire Dashboard opens automatically

## Performance Tips

### Development

- Use `AddDbContextPool` instead of `AddDbContext` for better performance
- Enable response compression in API Service for large payloads
- Use `AsNoTracking()` for read-only EF Core queries

### Database

- Ensure indexes are created (automatically via migrations)
- Monitor query performance in Aspire Dashboard
- Use `AsSplitQuery()` for multiple includes to avoid cartesian explosion

### SignalR

- Limit broadcast frequency (debounce rapid updates)
- Use `Clients.Others` to avoid echo back to sender
- Enable connection compression for slower networks

## Next Steps

After successful setup:

1. **Review Documentation**:
   - [data-model.md](data-model.md) - Entity schemas and relationships
   - [research.md](research.md) - Technology decisions and rationale
   - [contracts/](contracts/) - OpenAPI specifications for all APIs

2. **Explore Aspire Dashboard**:
   - View distributed traces for request flows
   - Monitor health checks and metrics
   - Inspect logs from all services in one place

3. **Run Tests**:
   ```bash
   dotnet test
   ```

4. **Generate Tasks**:
   - Use `/speckit.tasks` command to generate implementation task list

## Getting Help

- **Documentation**: See [plan.md](plan.md) for architecture details
- **API Reference**: Visit https://localhost:7001/swagger
- **Constitution**: Review [.specify/memory/constitution.md](../../.specify/memory/constitution.md) for coding standards

## Appendix: Port Reference

| Service | HTTP | HTTPS | Purpose |
|---------|------|-------|---------|
| Blazor Server | 5000 | 7000 | Frontend UI |
| API Service | 5001 | 7001 | REST API |
| PostgreSQL | 5432 | - | Database |
| Aspire Dashboard | 18888 | 18889 | Monitoring |

## Appendix: Environment Variables

```bash
# Taskify.AppHost/.env (create if needed)
ASPIRE_DASHBOARD_PORT=18888
POSTGRES_PASSWORD=postgres
API_ENVIRONMENT=Development
```

---

**Setup Complete!** You should now have Taskify MVP running locally. Navigate to https://localhost:7000 to start using the application.
