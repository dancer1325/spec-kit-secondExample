# Taskify MVP - Quick Start Guide

**Status**: Ready to execute (requires .NET 8.0 SDK)

## Prerequisites

```bash
# 1. Install .NET 8.0 SDK
brew install --cask dotnet-sdk

# 2. Verify installation
dotnet --version  # Should show 8.0.x

# 3. Install Aspire workload
dotnet workload install aspire

# 4. Verify Docker is running
docker ps
```

## Execute Implementation

```bash
# Navigate to repository root
cd /Users/alfredo.toledano/Projects/ai/spec-kit-secondExample

# Step 1: Run Phase 1 (Setup)
./setup-phase1.sh
# Creates solution, projects, adds packages
# ~5 minutes

# Step 2: Run Phase 2 (Foundational)
./deploy-phase2-files.sh
# Deploys code templates, creates migration
# ~2 minutes

# Step 3: Start application
dotnet run --project Taskify.AppHost
# Starts all services
```

## What Gets Created

### Projects
- `Taskify.sln` - Solution file
- `Taskify.AppHost/` - Aspire orchestration
- `Taskify.ServiceDefaults/` - Shared configuration
- `Taskify.Web/` - Blazor Server frontend
- `Taskify.ApiService/` - REST API backend

### Database
- PostgreSQL container (automatic)
- 5 users (1 PM, 4 Engineers)
- 3 projects
- 20 tasks

## Verify Success

```bash
# Build succeeds
dotnet build  # Should exit with code 0

# Application starts
dotnet run --project Taskify.AppHost
# Opens Aspire dashboard

# Check endpoints
curl http://localhost:{api-port}/health  # Should return 200 OK
```

## Troubleshooting

**dotnet: command not found**
→ Install .NET SDK (see Prerequisites)

**Aspire workload not found**
→ Run: `dotnet workload install aspire`

**Docker not running**
→ Start Docker Desktop application

**Port already in use**
→ Aspire will assign different ports automatically

## Documentation

- `SETUP.md` - Detailed prerequisites guide
- `IMPLEMENTATION_STATUS.md` - Complete status report
- `EXECUTION_SUMMARY.md` - What was accomplished
- `specs/001-create-taskify/` - Full specifications

## Next Steps After Phase 2

Once Phase 1 & 2 are complete:

1. ✅ Foundation ready
2. Implement User Story 0 (User selection) - 10 tasks
3. Implement User Story 1 (Kanban board) - 16 tasks
4. Implement User Story 2 (Task creation) - 16 tasks
5. Implement User Story 3 (Projects) - 13 tasks
6. Implement User Story 4 (Comments) - 21 tasks

See `specs/001-create-taskify/tasks.md` for complete task breakdown.

## Support

- All code templates in `templates/phase2/`
- Automation scripts: `setup-phase1.sh`, `deploy-phase2-files.sh`
- Configuration: `.gitignore`, `.editorconfig`

**Total execution time**: ~7 minutes (after prerequisites)
