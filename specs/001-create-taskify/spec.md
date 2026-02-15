# Feature Specification: Create Taskify MVP

**Feature Branch**: `001-create-taskify`
**Created**: 2026-02-15
**Status**: Draft
**Input**: User description: "Develop Taskify, a team productivity platform with projects, tasks, team members, comments, and Kanban-style boards"

## Clarifications

### Session 2026-02-15

- Q: When a user clicks or selects a task card to view its full details (description, comments, assignee, status controls), how should this detailed view be displayed? → A: Modal popup overlay (appears centered over the board with darkened background)
- Q: How should users change a task's status when viewing it in the modal popup? → A: Dropdown selector showing all four statuses (To Do, In Progress, In Review, Done)
- Q: How should users change a task's assignee when viewing it in the modal popup? → A: Dropdown selector showing all five predefined users (1 PM, 4 engineers)
- Q: How should users add a new comment when viewing the task detail modal? → A: Text input field with submit button at bottom of comment thread
- Q: When viewing a task in the detail modal, should users be able to edit the task's description after it has been created? → A: Yes, click description to edit inline with save/cancel buttons
- Q: What is the application launch and navigation flow? → A: Launch shows user selection list (5 users, no password) → Click user → Project list view → Click project → Kanban board
- Q: How are tasks assigned to the current user visually differentiated from other tasks? → A: Tasks assigned to the currently selected user are displayed in a different color on the board
- Q: Can users edit comments made by other users? → A: No, users can only edit their own comments, not others'
- Q: Can users delete comments made by other users? → A: No, users can only delete their own comments, not others'
- Q: When a user clicks "edit" on their own comment, how should the editing interface work? → A: Inline editing - comment text becomes editable field with save/cancel buttons in place
- Q: When a user clicks "delete" on their own comment, what confirmation mechanism should be used? → A: Confirmation dialog/modal asking "Delete this comment?" with Yes/Cancel buttons
- Q: For tasks assigned to the currently selected user, what level of visual differentiation should be used? → A: Distinct accent color with subtle background tint (clear but not overwhelming)
- Q: When viewing a Kanban board, what navigation mechanism should be provided to return to the project list? → A: Breadcrumb navigation showing "Projects > [Project Name]" with clickable "Projects" link
- Q: After selecting identity at launch, can users switch to a different user identity during the same session? → A: Can switch - user selector available in header/navigation menu throughout application

## User Scenarios & Testing *(mandatory)*

### User Story 0 - User Selection & Navigation (Priority: P0)

Team members need to select their identity when launching the application and navigate between projects to access their work.

**Why this priority**: This is the foundational entry point to the application. Without user selection, the system cannot establish context for personalization (showing "my tasks" in different color, comment permissions). It must be implemented before any other feature can function properly.

**Independent Test**: Can be fully tested by launching the application, selecting a user from the list of five, viewing the project list, selecting a project, and verifying the Kanban board loads. Delivers the value of application access and navigation.

**Acceptance Scenarios**:

1. **Given** a user launches Taskify, **When** the application loads, **Then** they see a selection screen with the five predefined users (1 PM, 4 engineers) with no password required
2. **Given** the user selection screen is displayed, **When** a user clicks on one of the five users, **Then** they navigate to the project list view
3. **Given** a user has selected their identity, **When** they view any board, **Then** tasks assigned to them display with a distinct accent color and subtle background tint, making them clearly different from tasks assigned to others
4. **Given** a user is viewing the project list, **When** they click on a project, **Then** they navigate to that project's Kanban board and see breadcrumb navigation showing "Projects > [Project Name]"
5. **Given** a user is viewing a Kanban board, **When** they click the "Projects" link in the breadcrumb navigation, **Then** they return to the project list view
6. **Given** a user is viewing any screen after initial login, **When** they click the user selector in the header/navigation menu, **Then** they see a dropdown or menu with all five predefined users
7. **Given** a user selector menu is open, **When** they select a different user, **Then** the current user context switches immediately and all personalization updates (task colors, comment permissions)

---

### User Story 1 - Kanban Board View & Task Movement (Priority: P1)

Team members need to visualize work in progress and move tasks through their workflow stages to track project progress.

**Why this priority**: This is the core value proposition of Taskify - visual task management. Without this, the platform has no fundamental purpose. It demonstrates the primary interaction model and delivers immediate value.

**Independent Test**: Can be fully tested by opening a project, viewing its Kanban board with tasks in different columns, and dragging a task from one column to another. Delivers the value of visual workflow management.

**Acceptance Scenarios**:

1. **Given** a user views a project, **When** they access the project board, **Then** they see tasks organized in four columns: "To Do", "In Progress", "In Review", and "Done"
2. **Given** tasks exist in the "To Do" column, **When** a user drags a task to "In Progress", **Then** the task moves to the new column and its status updates
3. **Given** a task is in "In Review", **When** a user drags it to "Done", **Then** the task moves to the Done column and its status reflects completion
4. **Given** a task detail modal is open, **When** a user selects a different status from the dropdown (e.g., changes from "To Do" to "In Progress"), **Then** the modal updates to show the new status and the task moves to the corresponding column on the board
5. **Given** multiple tasks exist across different columns, **When** a user views the board, **Then** all tasks display with their title, assignee, and current status

---

### User Story 2 - Task Creation & Assignment (Priority: P2)

Team members need to create new tasks and assign them to specific team members to distribute work and track ownership.

**Why this priority**: Task creation is essential for ongoing platform use. While story 1 shows existing tasks, this enables teams to add new work items, making the platform practical for real work.

**Independent Test**: Can be tested by creating a new task with a title and description, assigning it to one of the five predefined users, and verifying it appears in the "To Do" column. Delivers the value of work distribution.

**Acceptance Scenarios**:

1. **Given** a user is viewing a project board, **When** they create a new task with a title and description, **Then** the task appears in the "To Do" column
2. **Given** a task creation form is open, **When** a user selects an assignee from the five predefined users (1 PM, 4 engineers), **Then** the task shows the assigned person's name
3. **Given** a task exists on the board, **When** a user clicks the task card, **Then** a modal popup opens centered over the board showing the task title, description, assignee, and current status
4. **Given** a task detail modal is open, **When** a user clicks on the task description, **Then** the description becomes editable with save and cancel buttons appearing
5. **Given** a task description is being edited, **When** a user modifies the text and clicks save, **Then** the description updates and returns to read-only view
6. **Given** a task description is being edited, **When** a user clicks cancel, **Then** the original description is restored and returns to read-only view
7. **Given** a task detail modal is open, **When** a user selects a different assignee from the dropdown (e.g., changes from Engineer 1 to Engineer 2), **Then** the modal updates to show the new assignee and the task card on the board displays the new assignee name
8. **Given** a task detail modal is open, **When** a user closes the modal, **Then** they return to the board view with the modal dismissed
9. **Given** multiple tasks are assigned to different team members, **When** viewing the board, **Then** each task clearly displays who is responsible for it

---

### User Story 3 - Project Management (Priority: P3)

Users need to create and organize multiple projects to separate different initiatives and view all active projects.

**Why this priority**: Project organization enables teams to manage multiple concurrent initiatives. While less critical than task management, it provides necessary structure for real-world usage with multiple workstreams.

**Independent Test**: Can be tested by viewing the list of three sample projects, creating a new project, and navigating between different project boards. Delivers the value of multi-project organization.

**Acceptance Scenarios**:

1. **Given** a user accesses Taskify, **When** they view the projects list, **Then** they see all available projects including the three sample projects
2. **Given** the projects list is displayed, **When** a user creates a new project with a name and description, **Then** the new project appears in the list
3. **Given** multiple projects exist, **When** a user selects a project from the list, **Then** they navigate to that project's Kanban board
4. **Given** a project is selected, **When** a user views the project details, **Then** they see the project name, description, and associated team members

---

### User Story 4 - Task Collaboration (Priority: P4)

Team members need to communicate about tasks by adding comments to discuss details, ask questions, and share updates.

**Why this priority**: Comments enable asynchronous team collaboration. While important for communication, it's lower priority than core task management features and can be added after the basic workflow is functional.

**Independent Test**: Can be tested by selecting a task, adding a comment from a specific user, and viewing the comment thread. Delivers the value of task-specific team communication.

**Acceptance Scenarios**:

1. **Given** a user opens a task detail modal, **When** they enter text in the comment input field at the bottom and click the submit button, **Then** the comment appears in the task's comment thread with the user's name and timestamp
2. **Given** a comment has been submitted, **When** the comment appears in the thread, **Then** the text input field clears and is ready for another comment
3. **Given** multiple comments exist on a task, **When** a user views the task modal, **Then** they see all comments in chronological order with author and timestamp above the comment input field
4. **Given** a user views comments on a task, **When** they authored a comment, **Then** that comment displays edit and delete controls
5. **Given** a user views comments on a task, **When** another user authored a comment, **Then** that comment does NOT display edit or delete controls
6. **Given** a user clicks edit on their own comment, **When** the comment is clicked, **Then** the comment text becomes an editable field with save and cancel buttons appearing
7. **Given** a comment is being edited inline, **When** the user modifies the text and clicks save, **Then** the comment updates with the new text and returns to read-only view
8. **Given** a comment is being edited inline, **When** the user clicks cancel, **Then** the original comment text is restored and returns to read-only view
9. **Given** a user clicks delete on their own comment, **When** the delete button is clicked, **Then** a confirmation dialog appears asking "Delete this comment?" with Yes and Cancel buttons
10. **Given** a comment deletion confirmation dialog is displayed, **When** the user clicks Yes, **Then** the comment is removed from the thread and the dialog closes
11. **Given** a comment deletion confirmation dialog is displayed, **When** the user clicks Cancel, **Then** the comment is preserved and the dialog closes without deleting
8. **Given** a task has comments, **When** different users add comments, **Then** each comment clearly identifies which user wrote it
9. **Given** a user is viewing a task on the board, **When** the task has comments, **Then** a visual indicator shows that comments exist

---

### Edge Cases

- What happens when a user tries to move a task to the same column it's already in?
- How does the system handle creating a task without selecting an assignee?
- What happens when a project has no tasks?
- How does the system handle very long task titles or descriptions?
- What happens when all tasks are in the "Done" column?
- How are tasks ordered within a single column when multiple tasks exist?
- What happens to comment authorship tracking when a user switches identity mid-session? (Answer: AuthorId in database remains unchanged; only display permissions update based on new current user context)
- What happens when the same user opens Taskify in multiple browser tabs simultaneously? (Answer: Each tab maintains independent Blazor Server connection with synchronized SignalR updates for task movements and comments; user context can differ between tabs)

## Requirements *(mandatory)*

### Functional Requirements

- **FR-000**: System MUST display a user selection screen on application launch showing all five predefined users (1 PM, 4 engineers) with no password required
- **FR-000a**: System MUST establish user context when a user is selected from the user selection screen
- **FR-000b**: System MUST navigate from user selection screen to project list view when a user is clicked
- **FR-000c**: System MUST display tasks assigned to the currently selected user with a distinct accent color and subtle background tint to differentiate them from tasks assigned to other users on the Kanban board
- **FR-000d**: System MUST display breadcrumb navigation on the Kanban board showing "Projects > [Project Name]" with a clickable "Projects" link to return to the project list
- **FR-000e**: System MUST provide a user selector in the header/navigation menu that allows users to switch between any of the five predefined users at any time during the session
- **FR-000f**: System MUST update all personalization (task color highlighting, comment edit/delete permissions) immediately and synchronously when a different user is selected, with no confirmation required unless unsaved edits exist (see FR-017a)
- **FR-000g**: System MUST display the currently selected user's name and role in the header/navigation menu at all times
- **FR-001**: System MUST display four Kanban columns for each project board: "To Do", "In Progress", "In Review", and "Done"
- **FR-002**: System MUST support exactly five predefined users: one product manager and four engineers
- **FR-003**: System MUST initialize with three sample projects pre-populated with sample tasks
- **FR-004**: Users MUST be able to drag and drop tasks between any of the four status columns
- **FR-005**: System MUST update task status when moved to a different column
- **FR-006**: Users MUST be able to create new tasks with a title (required), description (optional), and assignee selection
- **FR-006a**: System MUST allow creating tasks without selecting an assignee (AssignedTo field is optional); unassigned tasks display "Unassigned" label on the card
- **FR-007**: System MUST display task assignee name on each task card in the Kanban view
- **FR-008**: Users MUST be able to create new projects with a name and description
- **FR-009**: System MUST provide a project list view showing all available projects
- **FR-010**: Users MUST be able to navigate from the project list to a specific project's Kanban board
- **FR-011**: Users MUST be able to add text comments to any task via a text input field with submit button located at the bottom of the comment thread in the task detail modal
- **FR-011a**: System MUST support unlimited number of comments per task
- **FR-011b**: Users MUST be able to edit their own comments via inline editing (comment text becomes editable field with save/cancel buttons) but MUST NOT be able to edit comments created by other users
- **FR-011c**: Users MUST be able to delete their own comments but MUST NOT be able to delete comments created by other users
- **FR-011d**: System MUST display edit and delete controls as icon buttons (pencil icon for edit, trash icon for delete) positioned to the right of each comment, visible only on comments authored by the currently selected user
- **FR-011d-1**: Permission checks for comment edit/delete MUST be enforced client-side by comparing comment AuthorId with current user context before displaying controls
- **FR-011d-2**: API endpoints MUST enforce permission checks server-side by validating authorId in request body matches comment AuthorId in database before allowing edit/delete operations
- **FR-011e**: System MUST persist comment edits when save is clicked and revert changes when cancel is clicked during inline comment editing
- **FR-011f**: System MUST display a confirmation dialog when a user attempts to delete their own comment, requiring explicit confirmation before deletion
- **FR-012**: System MUST display all comments for a task with author name and timestamp in chronological order
- **FR-013**: System MUST persist all changes (task movements, new tasks, new projects, comments) across browser sessions
- **FR-014**: System MUST support viewing task details in a modal popup overlay (centered over board with darkened background) showing full description, assignee, status, and comments
- **FR-014a**: System MUST provide a dropdown selector within the task detail modal allowing users to change task status to any of the four statuses (To Do, In Progress, In Review, Done)
- **FR-014b**: System MUST update the task's column position on the board when status is changed via the modal dropdown
- **FR-014c**: System MUST provide a dropdown selector within the task detail modal allowing users to change task assignee to any of the five predefined users
- **FR-014c-1**: Any user can reassign any task to any other user (no permission restrictions in MVP)
- **FR-014d**: System MUST update the task card's displayed assignee on the board when assignee is changed via the modal dropdown
- **FR-014e**: System MUST allow users to edit task description by clicking on it in the modal, enabling inline editing with save and cancel buttons
- **FR-014f**: System MUST persist task description changes when save button is clicked and revert changes when cancel button is clicked
- **FR-015**: System MUST NOT require passwords or traditional authentication; user identity is established through simple selection from the user list at application launch
- **FR-016**: System MUST validate all user inputs (task titles, project names, comment text) to prevent empty or malicious content per Security-First constitutional principle
- **FR-017**: System MUST persist current user selection across browser refresh/reload sessions using browser sessionStorage to maintain user context
- **FR-017a**: System MUST detect unsaved edits (task description being edited, comment being composed) when user attempts to switch identity and display confirmation dialog: "You have unsaved changes. Switch user anyway?" with Yes/Cancel buttons

### Key Entities

- **User**: Represents a team member with a name and role (Product Manager or Engineer). Five users are predefined: 1 PM and 4 Engineers. Users can be assigned to tasks. One user is selected as the "current user" at application launch, establishing the session context for personalization and permissions. The current user context is stored in UserContextService (Blazor Server in-memory state) and persisted to browser sessionStorage for refresh recovery. The current user can be switched at any time via the user selector in the header/navigation menu.

- **Project**: Represents a work initiative with a name, description, and associated tasks. Projects contain one Kanban board. System initializes with three sample projects.

- **Task**: Represents a work item with a title, optional description, assigned user, status (To Do/In Progress/In Review/Done), and associated comments. Tasks belong to exactly one project and appear on that project's board. Tasks assigned to the current user are visually differentiated with a different color.

- **Comment**: Represents a text message about a task with content, author (User), and timestamp. Comments belong to a specific task and enable team communication. Comments track authorship via AuthorId foreign key to Users table. Each comment stores the User ID who created it, enabling permission checks: users can only edit/delete comments where AuthorId matches their current user ID. Comment authorship is immutable and persists independently of user context switching.

- **Kanban Board**: Represents the visual organization of a project's tasks into four status columns. Each project has exactly one board.

- **Status Column**: Represents one of four workflow stages (To Do, In Progress, In Review, Done) that contains tasks. Columns are predefined and cannot be added or removed in this MVP.

## UI Specifications

### Task Card Styling

**My Tasks (Assigned to Current User)**:
- Primary accent color: `#1976D2` (Material Blue 700)
- Background tint: `rgba(25, 118, 210, 0.08)` (8% opacity)
- Border: `2px solid #1976D2`
- Contrast ratio: Minimum 4.5:1 for text on background (WCAG AA compliance)

**Other Tasks (Assigned to Different Users)**:
- Background: `#FFFFFF` (white)
- Border: `1px solid #E0E0E0` (light gray)
- Text color: `#212121` (dark gray)

**Unassigned Tasks**:
- Background: `#FAFAFA` (off-white)
- Border: `1px dashed #BDBDBD` (medium gray, dashed)
- Assignee label: Display "Unassigned" in `#757575` (gray 600)

### Comment Control Styling

**Edit/Delete Buttons** (visible only on current user's comments):
- Edit button: Pencil icon, size 16px, color `#1976D2`
- Delete button: Trash icon, size 16px, color `#D32F2F` (red)
- Position: Aligned to right side of comment, appearing on hover
- Spacing: 8px between buttons

**Comment Author Display**:
- Current user's comments: Author name in bold `#1976D2`
- Other users' comments: Author name in regular weight `#212121`

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Users can view a project's Kanban board and identify task status within 5 seconds of opening the project
- **SC-002**: Users can successfully move a task from one column to another with a single drag-and-drop interaction in under 3 seconds
- **SC-003**: Users can create a new task and see it appear on the board within 10 seconds
- **SC-004**: System displays all three sample projects with at least 5 tasks each distributed across different status columns
- **SC-005**: 100% of task movements persist across browser sessions (user closes and reopens browser, task remains in new status)
- **SC-006**: Users can navigate between different project boards and view distinct tasks for each project
- **SC-007**: All five predefined users are selectable when assigning tasks
- **SC-008**: Users can add comments to any task and view the complete comment thread
- **SC-009**: 95% of user interactions (task creation, movement, commenting) complete successfully without errors
- **SC-010**: System displays task details (title, assignee, status) clearly enough that users can identify task ownership without additional clicks

### Assumptions

- Browser compatibility: Modern browsers (Chrome, Firefox, Safari, Edge) within the last 2 major versions
- Data persistence: PostgreSQL 16 database with Entity Framework Core for server-side persistence, ensuring data integrity and enabling future multi-user collaboration
- Sample data: The three sample projects should represent typical project types (e.g., "Website Redesign", "Mobile App Development", "Marketing Campaign") with realistic task distributions, seeded via EF Core migrations
- User roles: The distinction between Product Manager and Engineer is for demonstration purposes and does not affect permissions (all users have the same capabilities in this MVP)
- User session: Single browser session per user is primary use case; Blazor Server maintains server-side state per SignalR connection
- Network architecture: Blazor Server frontend communicates with REST API backend; SignalR provides real-time updates for task movements and comments
- Task ordering: Within each column, tasks are ordered by Position field (integer) to support drag-and-drop reordering
- Empty states: When no tasks exist in a column, display an empty state message indicating users can add tasks
