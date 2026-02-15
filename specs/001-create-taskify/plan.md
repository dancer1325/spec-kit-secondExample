# Implementation Plan: Create Taskify MVP

**Branch**: `001-create-taskify` | **Date**: 2026-02-15 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/001-create-taskify/spec.md`

## Summary

Taskify is a team productivity platform providing visual Kanban-style task management with user context awareness, task assignment, and real-time collaboration through comments. The MVP delivers four core user stories: user selection & navigation (P0), Kanban board view & task movement (P1), task creation & assignment (P2), project management (P3), and task collaboration (P4).

**Technical Approach**: Built using .NET Aspire orchestration framework with Blazor Server for real-time interactive UI, Postgres database for persistence, and REST APIs for projects, tasks, and comments. Architecture separates concerns into Projects API, Tasks API, and Comments API services while Blazor Server provides server-side rendering with SignalR for real-time drag-and-drop interactions.

## Technical Context

**Language/Version**: C# / .NET 8.0 (LTS)
**Primary Dependencies**: .NET Aspire 8.0, Blazor Server, SignalR, Npgsql (Postgres client), Entity Framework Core 8.0
**Storage**: PostgreSQL 16+ (primary database)
**Testing**: xUnit, bUnit (Blazor component testing), Testcontainers (integration tests)
**Target Platform**: Linux/Windows server, containerized deployment via .NET Aspire
**Project Type**: Web application (Blazor Server frontend + REST APIs backend)
**Performance Goals**: <200ms API response time (p95), support 100 concurrent users with real-time updates, <3s task drag-and-drop interaction
**Constraints**: Single-user browser session primary use case (MVP), client-side only user selection (no authentication), real-time updates via SignalR required for task movements
**Scale/Scope**: 5 predefined users, 3 sample projects, ~15-20 sample tasks, unlimited comments per task

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### I. Security-First (NON-NEGOTIABLE)

✅ **PASS** - All API endpoints will implement input validation using Data Annotations and FluentValidation
- Task title/description: length limits, XSS sanitization
- Project names: length limits, special character validation
- Comment text: length limits, XSS sanitization
- User selection: enum validation (5 predefined users only)
- Status transitions: enum validation (4 states only)

⚠️ **DEFERRED** - Authentication/authorization deferred to post-MVP (FR-015 specifies no authentication for MVP)

### II. Microservices Architecture

⚠️ **REQUIRES JUSTIFICATION** - Three separate API services (Projects, Tasks, Notifications) proposed
- **Concern**: Constitution encourages modular monolith first, extract services when scaling demands
- **For MVP**: Consider single API backend with logical separation instead of physical microservices
- **Decision Required**: See Complexity Tracking section below

### III. Full Documentation

✅ **PASS** - Plan includes:
- README per service with setup instructions
- OpenAPI/Swagger documentation for all REST endpoints
- Inline XML documentation for public APIs
- Architecture Decision Records for key choices
- quickstart.md for developer onboarding

### IV. Test-First

✅ **PASS** - TDD workflow enforced:
- Unit tests for business logic (task state transitions, validation)
- Component tests for Blazor UI (bUnit)
- Integration tests for API contracts (Testcontainers + Postgres)
- E2E tests for critical user journeys

### V. Integration Testing

✅ **PASS** - Integration test strategy:
- API contract tests for Projects/Tasks/Notifications endpoints
- Database integration tests using Testcontainers
- Blazor Server integration tests with SignalR
- End-to-end tests for multi-user comment scenarios

### Gates Summary

| Principle | Status | Notes |
|-----------|--------|-------|
| Security-First | ✅ PASS | Input validation strategy defined; auth deferred per spec |
| Microservices | ⚠️ REVIEW | See Complexity Tracking - may simplify to modular monolith |
| Documentation | ✅ PASS | Documentation artifacts planned |
| Test-First | ✅ PASS | TDD workflow with xUnit/bUnit/Testcontainers |
| Integration Testing | ✅ PASS | Contract and E2E test strategy defined |

**GATE DECISION**: ⚠️ CONDITIONAL PASS - Proceed to Phase 0 research, revisit microservices decomposition decision in Phase 1 design based on .NET Aspire best practices research.

## Project Structure

### Documentation (this feature)

```text
specs/001-create-taskify/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (technology decisions, patterns)
├── data-model.md        # Phase 1 output (entities, schemas, relationships)
├── quickstart.md        # Phase 1 output (developer setup guide)
├── contracts/           # Phase 1 output (OpenAPI specs)
│   ├── projects-api.yaml
│   ├── tasks-api.yaml
│   └── comments-api.yaml
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
Taskify.sln

Taskify.AppHost/                    # .NET Aspire orchestration
├── Program.cs                       # Service discovery, container orchestration
├── appsettings.json
└── Taskify.AppHost.csproj

Taskify.ServiceDefaults/             # Shared Aspire service configuration
├── Extensions.cs                    # Health checks, OpenTelemetry, resilience
└── Taskify.ServiceDefaults.csproj

Taskify.Web/                         # Blazor Server application
├── Components/
│   ├── Pages/
│   │   ├── Index.razor              # User selection screen
│   │   ├── Projects.razor           # Project list view
│   │   └── Board.razor              # Kanban board view
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Shared/
│   │   ├── TaskCard.razor           # Draggable task card component
│   │   ├── TaskModal.razor          # Task detail modal
│   │   ├── CommentThread.razor      # Comment list component
│   │   └── UserSelector.razor       # Header user switcher
│   └── App.razor
├── Services/
│   ├── TaskifyApiClient.cs          # HTTP client for REST APIs
│   └── UserContextService.cs        # Current user state management
├── Models/
│   ├── UserContext.cs
│   ├── ProjectViewModel.cs
│   └── TaskViewModel.cs
├── wwwroot/
│   ├── css/
│   ├── js/
│   │   └── dragdrop.js              # Drag-and-drop interop
│   └── app.css
├── Program.cs
├── appsettings.json
└── Taskify.Web.csproj

Taskify.ApiService/                  # REST API backend
├── Controllers/
│   ├── ProjectsController.cs        # Projects CRUD endpoints
│   ├── TasksController.cs           # Tasks CRUD + status update endpoints
│   └── CommentsController.cs        # Comment CRUD endpoints
├── Models/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Project.cs
│   │   ├── Task.cs
│   │   └── Comment.cs
│   ├── DTOs/
│   │   ├── ProjectDto.cs
│   │   ├── TaskDto.cs
│   │   └── CommentDto.cs
│   └── Requests/
│       ├── CreateProjectRequest.cs
│       ├── CreateTaskRequest.cs
│       └── CreateCommentRequest.cs
├── Data/
│   ├── TaskifyDbContext.cs
│   └── Migrations/
├── Services/
│   ├── ProjectService.cs
│   ├── TaskService.cs
│   └── CommentService.cs
├── Validation/
│   ├── CreateProjectValidator.cs
│   ├── CreateTaskValidator.cs
│   └── CreateCommentValidator.cs
├── Program.cs
├── appsettings.json
└── Taskify.ApiService.csproj

Tests/
├── Taskify.Web.Tests/               # Blazor component tests
│   ├── Components/
│   │   ├── TaskCardTests.cs
│   │   ├── TaskModalTests.cs
│   │   └── BoardTests.cs
│   └── Services/
│       └── UserContextServiceTests.cs
├── Taskify.ApiService.Tests/        # API unit and integration tests
│   ├── Controllers/
│   │   ├── ProjectsControllerTests.cs
│   │   ├── TasksControllerTests.cs
│   │   └── CommentsControllerTests.cs
│   ├── Services/
│   │   ├── ProjectServiceTests.cs
│   │   ├── TaskServiceTests.cs
│   │   └── CommentServiceTests.cs
│   └── Integration/
│       ├── ProjectsApiIntegrationTests.cs
│       ├── TasksApiIntegrationTests.cs
│       └── DatabaseIntegrationTests.cs
└── Taskify.E2E.Tests/               # End-to-end tests
    ├── UserJourneys/
    │   ├── UserSelectionFlowTests.cs
    │   ├── TaskMovementFlowTests.cs
    │   └── CommentCollaborationTests.cs
    └── Fixtures/
        └── TestcontainersFixture.cs
```

**Structure Decision**: Using .NET Aspire web application structure with separate projects for:
- **Taskify.AppHost**: Orchestrates all services, manages service discovery, container lifecycle
- **Taskify.ServiceDefaults**: Shared configuration (health checks, telemetry, resilience patterns)
- **Taskify.Web**: Blazor Server frontend with SignalR for real-time updates
- **Taskify.ApiService**: REST API backend (initially monolithic, logically separated by controller)
- **Tests**: Comprehensive test coverage with xUnit, bUnit, Testcontainers

This structure follows .NET Aspire best practices while maintaining clean separation of concerns. The API is initially a monolithic service with logical boundaries (Controllers) that can be extracted to separate microservices post-MVP if scaling demands warrant it.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| Microservices decomposition (Constitution §II) | User input requested "Projects API, Tasks API, Notifications API" as separate services | **RECOMMENDED SIMPLIFICATION**: Start with single Taskify.ApiService backend with three controllers (ProjectsController, TasksController, NotificationsController) instead of three physical microservices. This aligns with constitution's "Start with modular monolith if appropriate, then extract services when scalability or team structure demands it." For MVP with 5 users and 3 projects, physical service separation adds deployment complexity without scalability benefits. Can extract to microservices post-MVP if needed. |
| Single-user session (not true microservices statelessness) | MVP scope explicitly limits to single-user browser session (Spec Assumptions) | Multi-user real-time collaboration deferred to post-MVP. Constitution acknowledges MVP constraints. Blazor Server's SignalR maintains connection state per client, which is acceptable for MVP. |

**DECISION**: Proceed with modular monolith approach - single Taskify.ApiService with three logical API boundaries (ProjectsController, TasksController, NotificationsController). This satisfies user's requirement for "Projects API, Tasks API, and Notifications API" as logical API contracts while avoiding premature microservices decomposition.

---

## Phase Completion Status

### Phase 0: Research ✅ COMPLETE

**Artifacts Generated**:
- `research.md` - Technology decisions, patterns, and Architecture Decision Records

**Key Decisions**:
1. .NET Aspire 8.0 for orchestration (over Docker Compose, Kubernetes directly)
2. Blazor Server with MudBlazor for drag-and-drop UI (over Blazorise, SortableJS)
3. PostgreSQL 16 with EF Core 8.0 (over SQL Server, SQLite)
4. FluentValidation + Data Annotations for input validation
5. xUnit + bUnit + Testcontainers for testing
6. Modular monolith architecture (single API service with three logical controllers)

### Phase 1: Design & Contracts ✅ COMPLETE

**Artifacts Generated**:
- `data-model.md` - Entity schemas, relationships, indexes, validation rules
- `contracts/projects-api.yaml` - OpenAPI specification for Projects API (6 endpoints)
- `contracts/tasks-api.yaml` - OpenAPI specification for Tasks API (9 endpoints)
- `contracts/comments-api.yaml` - OpenAPI specification for Comments API (5 endpoints)
- `quickstart.md` - Developer onboarding guide with setup instructions

**Data Model Summary**:
- 4 core entities: User, Project, Task, Comment
- 20 sample tasks seeded across 3 projects
- Composite indexes for Kanban board performance
- Soft delete strategy for comments

**API Summary**:
- **Projects API**: CRUD operations for projects
- **Tasks API**: Task management with drag-and-drop status updates, assignment, and filtering
- **Notifications API**: Comment CRUD with permission enforcement (users can only edit/delete own comments)
- All APIs return standard ErrorResponse format with validation details

### Post-Phase 1 Constitution Re-Check ✅ PASS

| Principle | Status | Notes |
|-----------|--------|-------|
| Security-First | ✅ PASS | FluentValidation configured for all request DTOs; XSS prevention via HTML encoding |
| Microservices | ✅ PASS | Modular monolith approved (Complexity Tracking justification accepted) |
| Documentation | ✅ PASS | OpenAPI specs, data model, quickstart guide, and ADRs complete |
| Test-First | ✅ PASS | Test structure defined in project layout; Testcontainers for integration tests |
| Integration Testing | ✅ PASS | Contract tests via OpenAPI schemas; E2E tests planned for user journeys |

**Final Gate Decision**: ✅ **APPROVED FOR IMPLEMENTATION**

All constitutional principles satisfied. Proceed to **Phase 2: Task Decomposition** via `/speckit.tasks` command.

---

## Next Steps

1. **Generate Tasks**: Run `/speckit.tasks` to create implementation task list from `tasks-template.md`
2. **Setup Development Environment**: Follow `quickstart.md` to install .NET Aspire, Docker, and dependencies
3. **Begin Implementation**: Start with Phase 1 (Setup) tasks, then Foundational tasks, then user stories in priority order (P0 → P1 → P2 → P3 → P4)

**Recommended Task Execution Order**:
1. Setup (Project structure, dependencies)
2. Foundational (Database schema, service infrastructure, health checks)
3. User Story 0 (User selection, navigation, breadcrumbs)
4. User Story 1 (Kanban board view, drag-and-drop)
5. User Story 2 (Task creation, assignment, inline editing)
6. User Story 3 (Project management)
7. User Story 4 (Comments with permission enforcement)
8. Polish (Documentation, performance optimization, E2E tests)
