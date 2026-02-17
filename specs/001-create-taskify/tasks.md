---

description: "Task list for Create Taskify MVP implementation"
---

# Tasks: Create Taskify MVP

**Input**: Design documents from `/specs/001-create-taskify/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: Tests are included per constitutional requirement (Test-First, §IV). Tests must be written and fail before implementation.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US0, US1, US2, US3, US4)
- Include exact file paths in descriptions

## Path Conventions

- **Solution root**: `Taskify.sln`
- **AppHost**: `Taskify.AppHost/`
- **ServiceDefaults**: `Taskify.ServiceDefaults/`
- **Blazor Server**: `Taskify.Web/`
- **API Service**: `Taskify.ApiService/`
- **Tests**: `Tests/` (Taskify.Web.Tests, Taskify.ApiService.Tests, Taskify.E2E.Tests)

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure per .NET Aspire best practices

- [x] T001 Create solution file Taskify.sln in repository root
- [x] T002 [P] Create Taskify.AppHost project with .NET Aspire workload in Taskify.AppHost/
- [x] T003 [P] Create Taskify.ServiceDefaults project for shared configuration in Taskify.ServiceDefaults/
- [x] T004 [P] Create Taskify.Web Blazor Server project in Taskify.Web/
- [x] T005 [P] Create Taskify.ApiService ASP.NET Core Web API project in Taskify.ApiService/
- [x] T006 [P] Add MudBlazor package to Taskify.Web/Taskify.Web.csproj
- [x] T007 [P] Add Npgsql.EntityFrameworkCore.PostgreSQL package to Taskify.ApiService/Taskify.ApiService.csproj
- [x] T008 [P] Add FluentValidation.AspNetCore package to Taskify.ApiService/Taskify.ApiService.csproj
- [x] T009 [P] Configure linting and formatting tools (.editorconfig in repository root)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T010 Configure Aspire AppHost to orchestrate Postgres, API, and Blazor in Taskify.AppHost/Program.cs
- [ ] T011 [P] Implement ServiceDefaults extensions for health checks and telemetry in Taskify.ServiceDefaults/Extensions.cs
- [ ] T012 [P] Create User entity model in Taskify.ApiService/Models/Entities/User.cs
- [ ] T013 [P] Create Project entity model in Taskify.ApiService/Models/Entities/Project.cs
- [ ] T014 [P] Create Task entity model in Taskify.ApiService/Models/Entities/Task.cs
- [ ] T015 [P] Create Comment entity model in Taskify.ApiService/Models/Entities/Comment.cs
- [ ] T016 Create TaskifyDbContext with entity configurations and indexes in Taskify.ApiService/Data/TaskifyDbContext.cs
- [ ] T017 Seed database with 5 users, 3 projects, and 20 tasks in TaskifyDbContext.OnModelCreating
- [ ] T018 Create initial EF Core migration in Taskify.ApiService/Data/Migrations/
- [ ] T019 [P] Implement UserContextService for current user state management in Taskify.Web/Services/UserContextService.cs
- [ ] T020 [P] Implement TaskifyApiClient HTTP client service in Taskify.Web/Services/TaskifyApiClient.cs
- [ ] T021 [P] Configure Program.cs for Aspire service discovery and health checks in Taskify.ApiService/Program.cs
- [ ] T022 [P] Configure Program.cs for Blazor Server and MudBlazor in Taskify.Web/Program.cs

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 0 - User Selection & Navigation (Priority: P0) 🎯 MVP Foundation

**Goal**: Enable user identity selection, project list navigation, and breadcrumb navigation

**Independent Test**: Launch application, select a user from 5 predefined users, view project list, navigate to a project's Kanban board, return via breadcrumbs

### Tests for User Story 0

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T023 [US0] Contract test for user context establishment in Tests/Taskify.Web.Tests/Services/UserContextServiceTests.cs
- [ ] T024 [US0] Integration test for user selection flow in Tests/Taskify.E2E.Tests/UserJourneys/UserSelectionFlowTests.cs
- [ ] T024a [US0] CHECKPOINT: Verify all US0 tests fail (RED state) before proceeding to implementation

### Implementation for User Story 0

- [ ] T025 [P] [US0] Create Index.razor user selection page in Taskify.Web/Components/Pages/Index.razor
- [ ] T026 [P] [US0] Create Projects.razor project list page in Taskify.Web/Components/Pages/Projects.razor
- [ ] T027 [P] [US0] Create UserSelector.razor header component in Taskify.Web/Components/Shared/UserSelector.razor
- [ ] T028 [P] [US0] Create MainLayout.razor with breadcrumb navigation in Taskify.Web/Components/Layout/MainLayout.razor
- [ ] T029 [US0] Implement user context state persistence across navigation in UserContextService.cs
- [ ] T030 [US0] Add task color highlighting based on current user in Board.razor CSS
- [ ] T031 [US0] Add user switching functionality to UserSelector dropdown with immediate personalization updates

**Checkpoint**: At this point, User Story 0 should be fully functional and testable independently

---

## Phase 4: User Story 1 - Kanban Board View & Task Movement (Priority: P1) 🎯 Core MVP

**Goal**: Display Kanban board with four columns, drag-and-drop task movement, real-time updates via SignalR

**Independent Test**: Open project board, see tasks in four columns, drag a task between columns, verify real-time updates

### Tests for User Story 1

- [ ] T032 [US1] Contract test for GET /projects/{id}/tasks endpoint in Tests/Taskify.ApiService.Tests/Controllers/TasksControllerTests.cs
- [ ] T033 [US1] Contract test for PATCH /tasks/{id}/status endpoint in Tests/Taskify.ApiService.Tests/Controllers/TasksControllerTests.cs
- [ ] T034 [US1] Integration test for task drag-and-drop flow in Tests/Taskify.E2E.Tests/UserJourneys/TaskMovementFlowTests.cs
- [ ] T035 [US1] Component test for TaskCard render in Tests/Taskify.Web.Tests/Components/TaskCardTests.cs
- [ ] T035a [US1] CHECKPOINT: Verify all US1 tests fail (RED state) before proceeding to implementation

### Implementation for User Story 1

- [ ] T036 [P] [US1] Create TaskDto data transfer object in Taskify.ApiService/Models/DTOs/TaskDto.cs
- [ ] T037 [P] [US1] Create TaskService for task business logic in Taskify.ApiService/Services/TaskService.cs
- [ ] T038 [US1] Implement GET /projects/{projectId}/tasks in TasksController.cs
- [ ] T039 [US1] Implement PATCH /tasks/{taskId}/status in TasksController.cs
- [ ] T040 [P] [US1] Create Board.razor Kanban board page in Taskify.Web/Components/Pages/Board.razor
- [ ] T041 [P] [US1] Create TaskCard.razor component with MudBlazor drag-drop in Taskify.Web/Components/Shared/TaskCard.razor
- [ ] T042 [US1] Implement MudDropContainer with four status columns in Board.razor
- [ ] T043 [US1] Add task color differentiation for current user's assigned tasks in TaskCard.razor
- [ ] T044 [US1] Implement KanbanHub SignalR hub for real-time task updates in Taskify.ApiService/Hubs/KanbanHub.cs
- [ ] T045 [US1] Connect Blazor client to SignalR hub in Board.razor OnInitializedAsync
- [ ] T046 [US1] Broadcast task movements via SignalR when status changes in TaskService.cs

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 5: User Story 2 - Task Creation & Assignment (Priority: P2)

**Goal**: Create new tasks, assign to users, edit task details inline, reassign tasks

**Independent Test**: Create new task with title/description, assign to user, edit task description inline, reassign to different user

### Tests for User Story 2

- [ ] T047 [US2] Contract test for POST /projects/{id}/tasks endpoint in Tests/Taskify.ApiService.Tests/Controllers/TasksControllerTests.cs
- [ ] T048 [US2] Contract test for PUT /tasks/{id} endpoint in Tests/Taskify.ApiService.Tests/Controllers/TasksControllerTests.cs
- [ ] T049 [US2] Contract test for PATCH /tasks/{id}/assign endpoint in Tests/Taskify.ApiService.Tests/Controllers/TasksControllerTests.cs
- [ ] T050 [US2] Component test for TaskModal component in Tests/Taskify.Web.Tests/Components/TaskModalTests.cs
- [ ] T050a [US2] CHECKPOINT: Verify all US2 tests fail (RED state) before proceeding to implementation

### Implementation for User Story 2

- [ ] T051 [P] [US2] Create CreateTaskRequest validation in Taskify.ApiService/Validation/CreateTaskValidator.cs
- [ ] T052 [P] [US2] Create UpdateTaskRequest validation in Taskify.ApiService/Validation/UpdateTaskValidator.cs
- [ ] T053 [US2] Implement POST /projects/{projectId}/tasks in TasksController.cs
- [ ] T054 [US2] Implement PUT /tasks/{taskId} in TasksController.cs
- [ ] T055 [US2] Implement PATCH /tasks/{taskId}/assign in TasksController.cs
- [ ] T056 [P] [US2] Create TaskModal.razor component with modal overlay in Taskify.Web/Components/Shared/TaskModal.razor
- [ ] T057 [US2] Add inline task description editing with save/cancel buttons in TaskModal.razor
- [ ] T058 [US2] Add assignee dropdown selector in TaskModal.razor
- [ ] T059 [US2] Add status dropdown selector in TaskModal.razor
- [ ] T060 [US2] Implement task creation button and form in Board.razor
- [ ] T061 [US2] Wire TaskModal to API endpoints for create, update, and assign operations

**Checkpoint**: At this point, User Stories 0, 1, AND 2 should all work independently

---

## Phase 6: User Story 3 - Project Management (Priority: P3)

**Goal**: View project list, create new projects, navigate between projects

**Independent Test**: View list of 3 sample projects, create new project, navigate to its board

### Tests for User Story 3

- [ ] T062 [US3] Contract test for GET /projects endpoint in Tests/Taskify.ApiService.Tests/Controllers/ProjectsControllerTests.cs
- [ ] T063 [US3] Contract test for POST /projects endpoint in Tests/Taskify.ApiService.Tests/Controllers/ProjectsControllerTests.cs
- [ ] T064 [US3] Contract test for GET /projects/{id} endpoint in Tests/Taskify.ApiService.Tests/Controllers/ProjectsControllerTests.cs
- [ ] T064a [US3] CHECKPOINT: Verify all US3 tests fail (RED state) before proceeding to implementation

### Implementation for User Story 3

- [ ] T065 [P] [US3] Create ProjectDto data transfer object in Taskify.ApiService/Models/DTOs/ProjectDto.cs
- [ ] T066 [P] [US3] Create CreateProjectRequest validation in Taskify.ApiService/Validation/CreateProjectValidator.cs
- [ ] T067 [P] [US3] Create ProjectService for project business logic in Taskify.ApiService/Services/ProjectService.cs
- [ ] T068 [US3] Implement GET /projects in ProjectsController.cs
- [ ] T069 [US3] Implement POST /projects in ProjectsController.cs
- [ ] T070 [US3] Implement GET /projects/{projectId} in ProjectsController.cs
- [ ] T071 [US3] Add project creation dialog to Projects.razor page
- [ ] T072 [US3] Display project list with task count in Projects.razor
- [ ] T073 [US3] Add navigation from project list to Board.razor

**Checkpoint**: All user stories 0-3 should now be independently functional

---

## Phase 7: User Story 4 - Task Collaboration (Priority: P4)

**Goal**: Add comments to tasks, edit own comments, delete own comments with confirmation

**Independent Test**: Open task, add comment, edit own comment inline, delete own comment with confirmation dialog

### Tests for User Story 4

- [ ] T074 [US4] Contract test for GET /tasks/{id}/comments endpoint in Tests/Taskify.ApiService.Tests/Controllers/CommentsControllerTests.cs
- [ ] T075 [US4] Contract test for POST /tasks/{id}/comments endpoint in Tests/Taskify.ApiService.Tests/Controllers/CommentsControllerTests.cs
- [ ] T076 [US4] Contract test for PUT /comments/{id} endpoint in Tests/Taskify.ApiService.Tests/Controllers/CommentsControllerTests.cs
- [ ] T077 [US4] Contract test for DELETE /comments/{id} endpoint in Tests/Taskify.ApiService.Tests/Controllers/CommentsControllerTests.cs
- [ ] T078 [US4] Integration test for comment collaboration flow in Tests/Taskify.E2E.Tests/UserJourneys/CommentCollaborationTests.cs
- [ ] T078a [US4] CHECKPOINT: Verify all US4 tests fail (RED state) before proceeding to implementation

### Implementation for User Story 4

- [ ] T079 [P] [US4] Create CommentDto data transfer object in Taskify.ApiService/Models/DTOs/CommentDto.cs
- [ ] T080 [P] [US4] Create CreateCommentRequest validation in Taskify.ApiService/Validation/CreateCommentValidator.cs
- [ ] T081 [P] [US4] Create CommentService for comment business logic in Taskify.ApiService/Services/CommentService.cs
- [ ] T082 [US4] Implement GET /tasks/{taskId}/comments in CommentsController.cs
- [ ] T083 [US4] Implement POST /tasks/{taskId}/comments in CommentsController.cs
- [ ] T084 [US4] Implement PUT /comments/{commentId} with authorship permission check in CommentsController.cs
- [ ] T085 [US4] Implement DELETE /comments/{commentId} with authorship permission check in CommentsController.cs
- [ ] T086 [P] [US4] Create CommentThread.razor component in Taskify.Web/Components/Shared/CommentThread.razor
- [ ] T087 [US4] Add comment list display with author and timestamp in CommentThread.razor
- [ ] T088 [US4] Add comment input field with submit button at bottom in CommentThread.razor
- [ ] T089 [US4] Show edit/delete controls only on current user's comments in CommentThread.razor
- [ ] T090 [US4] Implement inline comment editing with save/cancel buttons in CommentThread.razor
- [ ] T091 [US4] Implement delete confirmation dialog in CommentThread.razor
- [ ] T092 [US4] Integrate CommentThread into TaskModal.razor
- [ ] T093 [US4] Add SignalR broadcasting for new comments in CommentService.cs

**Checkpoint**: All user stories should now be independently functional

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T094 [P] Add XML documentation comments to all public APIs in Taskify.ApiService/Controllers/
- [ ] T095 [P] Configure Swagger/OpenAPI documentation in Taskify.ApiService/Program.cs
- [ ] T096 [P] Create README.md for Taskify.ApiService with setup instructions
- [ ] T097 [P] Create README.md for Taskify.Web with development guide
- [ ] T098 [P] Add global error handling middleware in Taskify.ApiService/Program.cs
- [ ] T099 [P] Add loading states and spinners to Blazor components in Taskify.Web/Components/Shared/
- [ ] T100 [P] Add empty state messages for boards with no tasks in Board.razor
- [ ] T101 [P] Optimize database queries with AsNoTracking for read-only operations in all services
- [ ] T102 [P] Add response compression middleware in Taskify.ApiService/Program.cs
- [ ] T103 [P] Implement debouncing for rapid SignalR updates in Board.razor
- [ ] T104 [P] Add unit tests for ProjectService in Tests/Taskify.ApiService.Tests/Services/ProjectServiceTests.cs
- [ ] T105 [P] Add unit tests for TaskService in Tests/Taskify.ApiService.Tests/Services/TaskServiceTests.cs
- [ ] T106 [P] Add unit tests for CommentService in Tests/Taskify.ApiService.Tests/Services/CommentServiceTests.cs
- [ ] T107 Run quickstart.md validation (verify all setup steps work correctly)
- [ ] T108 Add Architecture Decision Record (ADR) for modular monolith choice in docs/adr/001-modular-monolith.md
- [ ] T109 [P] Security test for XSS prevention in task titles and descriptions in Tests/Taskify.ApiService.Tests/Security/XssPreventionTests.cs
- [ ] T110 [P] Security test for XSS prevention in project names and descriptions in Tests/Taskify.ApiService.Tests/Security/XssPreventionTests.cs
- [ ] T111 [P] Security test for XSS prevention in comment content in Tests/Taskify.ApiService.Tests/Security/XssPreventionTests.cs
- [ ] T112 [P] Security test for length limit enforcement across all validated inputs in Tests/Taskify.ApiService.Tests/Security/ValidationSecurityTests.cs
- [ ] T113 [P] Security test for malicious script tag rejection in all text inputs in Tests/Taskify.ApiService.Tests/Security/ValidationSecurityTests.cs

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-7)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P0 → P1 → P2 → P3 → P4)
- **Polish (Phase 8)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 0 (P0)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - Depends on US0 for navigation context
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1 but independently testable
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - Depends on US0 for navigation flow
- **User Story 4 (P4)**: Can start after Foundational (Phase 2) - Integrates with US2 (TaskModal) but independently testable

### Within Each User Story

- Tests (if included) MUST be written and FAIL before implementation
- Models before services
- Services before controllers
- Controllers before UI components
- Core implementation before SignalR broadcasting
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel (T002-T009)
- All Foundational entity models marked [P] can run in parallel (T012-T015)
- Once Foundational phase completes, user stories US0, US1, US2, US3 can start in parallel (if team capacity allows)
- All tests for a user story marked [P] can run in parallel
- DTOs and validators within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together:
Task: T032 - Contract test for GET /projects/{id}/tasks
Task: T033 - Contract test for PATCH /tasks/{id}/status
Task: T034 - Integration test for task drag-and-drop
Task: T035 - Component test for TaskCard render

# Launch all parallel implementation tasks for User Story 1:
Task: T036 - Create TaskDto
Task: T041 - Create TaskCard.razor component
```

---

## Implementation Strategy

### MVP First (User Story 0 + 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 0 (User selection & navigation)
4. Complete Phase 4: User Story 1 (Kanban board & drag-drop)
5. **STOP and VALIDATE**: Test User Stories 0 and 1 independently
6. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 0 → Test independently → Foundation with navigation ✅
3. Add User Story 1 → Test independently → Core Kanban MVP ✅
4. Add User Story 2 → Test independently → Task creation & editing ✅
5. Add User Story 3 → Test independently → Multi-project support ✅
6. Add User Story 4 → Test independently → Team collaboration ✅
7. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 0 (P0)
   - Developer B: User Story 1 (P1) - after US0 navigation exists
   - Developer C: User Story 2 (P2)
   - Developer D: User Story 3 (P3)
3. Stories complete and integrate independently
4. User Story 4 can be added by any developer after US2 (TaskModal) exists

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence

---

## Task Count Summary

- **Total Tasks**: 118 (includes 5 RED checkpoints + 5 security tests added during analysis)
- **Setup (Phase 1)**: 9 tasks
- **Foundational (Phase 2)**: 13 tasks (CRITICAL - blocks all stories)
- **User Story 0 (P0)**: 10 tasks (2 tests + 1 RED checkpoint + 7 implementation)
- **User Story 1 (P1)**: 16 tasks (4 tests + 1 RED checkpoint + 11 implementation)
- **User Story 2 (P2)**: 16 tasks (4 tests + 1 RED checkpoint + 11 implementation)
- **User Story 3 (P3)**: 13 tasks (3 tests + 1 RED checkpoint + 9 implementation)
- **User Story 4 (P4)**: 21 tasks (5 tests + 1 RED checkpoint + 15 implementation)
- **Polish (Phase 8)**: 20 tasks (includes 5 security validation tests)

**Parallel Opportunities**: 42 tasks marked [P] can run in parallel within their phase (test tasks no longer marked [P] to enforce TDD)

**MVP Scope Recommendation**: Phases 1-4 (Setup + Foundational + US0 + US1) = 49 tasks for core Kanban functionality (includes RED checkpoints)
