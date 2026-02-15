# Data Model: Create Taskify MVP

**Date**: 2026-02-15
**Status**: Complete
**Database**: PostgreSQL 16
**ORM**: Entity Framework Core 8.0

## Overview

This document defines the complete data model for Taskify MVP, including entity schemas, relationships, validation rules, and database constraints. All entities implement auditing fields and support the four core user stories (P0-P4).

## Entity Relationship Diagram

```text
┌─────────────┐         ┌──────────────┐
│    User     │         │   Project    │
├─────────────┤         ├──────────────┤
│ Id (PK)     │         │ Id (PK)      │
│ Name        │         │ Name         │
│ Email       │         │ Description  │
│ Role        │         │ CreatedAt    │
│ CreatedAt   │         │ UpdatedAt    │
└──────┬──────┘         └───────┬──────┘
       │                        │
       │                        │
       │ AssignedTo (FK)        │ ProjectId (FK)
       │                        │
       └────────────┬───────────┘
                    │
              ┌─────▼──────┐
              │    Task    │
              ├────────────┤
              │ Id (PK)    │
              │ Title      │
              │ Description│
              │ Status     │
              │ Position   │
              │ ProjectId  │
              │ AssignedToId│
              │ CreatedAt  │
              │ UpdatedAt  │
              └─────┬──────┘
                    │
                    │ TaskId (FK)
                    │
              ┌─────▼──────┐
              │  Comment   │
              ├────────────┤
              │ Id (PK)    │
              │ Content    │
              │ TaskId (FK)│
              │ AuthorId(FK)│
              │ CreatedAt  │
              │ UpdatedAt  │
              │ IsDeleted  │
              └────────────┘
```

## Entities

### 1. User

Represents a team member with predefined roles. Five users are seeded for MVP (1 PM, 4 Engineers).

**Table Name**: `Users`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | `int` | PRIMARY KEY, IDENTITY | Auto-incrementing unique identifier |
| `Name` | `nvarchar(100)` | NOT NULL | User's full name |
| `Email` | `nvarchar(254)` | NOT NULL, UNIQUE | Email address (future auth use) |
| `Role` | `nvarchar(50)` | NOT NULL, CHECK IN ('ProductManager', 'Engineer') | User role |
| `CreatedAt` | `timestamp` | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Record creation time |

**Indexes**:
- `PRIMARY KEY (Id)`
- `UNIQUE INDEX IX_Users_Email (Email)`

**Relationships**:
- **One-to-Many** with `Tasks` (user can be assigned many tasks)
- **One-to-Many** with `Comments` (user can author many comments)

**Validation Rules**:
- `Name`: Required, 1-100 characters, no leading/trailing whitespace
- `Email`: Required, valid email format, unique
- `Role`: Must be "ProductManager" or "Engineer"

**Sample Data (Seeded)**:
```sql
INSERT INTO Users (Id, Name, Email, Role, CreatedAt) VALUES
(1, 'Sarah Chen', 'sarah.chen@taskify.com', 'ProductManager', '2026-02-15 00:00:00'),
(2, 'Marcus Rodriguez', 'marcus.rodriguez@taskify.com', 'Engineer', '2026-02-15 00:00:00'),
(3, 'Aisha Patel', 'aisha.patel@taskify.com', 'Engineer', '2026-02-15 00:00:00'),
(4, 'James Kim', 'james.kim@taskify.com', 'Engineer', '2026-02-15 00:00:00'),
(5, 'Emma Thompson', 'emma.thompson@taskify.com', 'Engineer', '2026-02-15 00:00:00');
```

**Entity Framework Core Model**:
```csharp
public class User
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(254), EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MaxLength(50)]
    public string Role { get; set; } = null!;  // "ProductManager" or "Engineer"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Task> AssignedTasks { get; } = new List<Task>();
    public ICollection<Comment> AuthoredComments { get; } = new List<Comment>();
}
```

---

### 2. Project

Represents a work initiative containing tasks. Three sample projects are seeded for MVP.

**Table Name**: `Projects`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | `int` | PRIMARY KEY, IDENTITY | Auto-incrementing unique identifier |
| `Name` | `nvarchar(200)` | NOT NULL | Project name |
| `Description` | `nvarchar(1000)` | NOT NULL | Project description |
| `CreatedAt` | `timestamp` | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Record creation time |
| `UpdatedAt` | `timestamp` | NULL | Last modification time |

**Indexes**:
- `PRIMARY KEY (Id)`
- `INDEX IX_Projects_Name (Name)` - for project list searches

**Relationships**:
- **One-to-Many** with `Tasks` (project contains many tasks)

**Validation Rules**:
- `Name`: Required, 1-200 characters, no HTML tags
- `Description`: Required, 1-1000 characters, no HTML tags

**Sample Data (Seeded)**:
```sql
INSERT INTO Projects (Id, Name, Description, CreatedAt) VALUES
(1, 'Website Redesign', 'Modernize company website UI/UX with responsive design and improved accessibility', '2026-02-15 00:00:00'),
(2, 'Mobile App Development', 'Build iOS and Android apps for customer engagement with offline sync capabilities', '2026-02-15 00:00:00'),
(3, 'Marketing Campaign', 'Q1 product launch campaign including social media, email marketing, and PR outreach', '2026-02-15 00:00:00');
```

**Entity Framework Core Model**:
```csharp
public class Project
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required, MaxLength(1000)]
    public string Description { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Task> Tasks { get; } = new List<Task>();
}
```

---

### 3. Task

Represents a work item with title, description, status, and assignment. Tasks belong to a project and can be assigned to a user.

**Table Name**: `Tasks`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | `int` | PRIMARY KEY, IDENTITY | Auto-incrementing unique identifier |
| `Title` | `nvarchar(200)` | NOT NULL | Task title |
| `Description` | `nvarchar(2000)` | NULL | Detailed task description (optional) |
| `Status` | `nvarchar(50)` | NOT NULL, CHECK IN ('To Do', 'In Progress', 'In Review', 'Done') | Current status |
| `Position` | `int` | NOT NULL, DEFAULT 0 | Position within status column (for ordering) |
| `ProjectId` | `int` | NOT NULL, FOREIGN KEY → Projects(Id) | Project this task belongs to |
| `AssignedToId` | `int` | NULL, FOREIGN KEY → Users(Id) | User assigned to this task (nullable) |
| `CreatedAt` | `timestamp` | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Record creation time |
| `UpdatedAt` | `timestamp` | NULL | Last modification time |

**Indexes**:
- `PRIMARY KEY (Id)`
- `INDEX IX_Tasks_ProjectId (ProjectId)` - for board queries
- `INDEX IX_Tasks_AssignedToId (AssignedToId)` - for user task lists
- **COMPOSITE INDEX** `IX_Tasks_Project_Status_Position (ProjectId, Status, Position)` - for Kanban board view
- **COMPOSITE INDEX** `IX_Tasks_AssignedTo_Status (AssignedToId, Status)` - for user's task filtering

**Relationships**:
- **Many-to-One** with `Project` (task belongs to one project)
- **Many-to-One** with `User` (task assigned to zero or one user)
- **One-to-Many** with `Comments` (task has many comments)

**Validation Rules**:
- `Title`: Required, 1-200 characters, no HTML tags
- `Description`: Optional, max 2000 characters, no script tags (XSS prevention)
- `Status`: Must be one of: "To Do", "In Progress", "In Review", "Done"
- `Position`: Non-negative integer
- `ProjectId`: Must reference existing project
- `AssignedToId`: If provided, must reference existing user (1-5 for MVP)

**State Transition Rules** (enforced in service layer):
```text
To Do → In Progress → In Review → Done
  ↓         ↓            ↓         ↓
 Any      Any          Any       Any  (can move backward)
```

**Sample Data (Seeded - 20 tasks across 3 projects)**:
```sql
-- Website Redesign Project (7 tasks)
INSERT INTO Tasks (Title, Description, Status, Position, ProjectId, AssignedToId, CreatedAt) VALUES
('Design new homepage', 'Create mockups for homepage with modern design and clear CTAs', 'In Progress', 0, 1, 2, '2026-02-15 00:00:00'),
('Implement navigation menu', 'Build responsive navigation with dropdown menus and mobile hamburger', 'To Do', 0, 1, 3, '2026-02-15 00:00:00'),
('Add contact form', 'Create contact form with validation and email notifications', 'To Do', 1, 1, NULL, '2026-02-15 00:00:00'),
('Optimize images', 'Compress all images and implement lazy loading', 'In Review', 0, 1, 4, '2026-02-15 00:00:00'),
('Setup analytics', 'Integrate Google Analytics and setup conversion tracking', 'Done', 0, 1, 2, '2026-02-15 00:00:00'),
('Write documentation', 'Document CMS usage and deployment process', 'In Progress', 1, 1, 5, '2026-02-15 00:00:00'),
('Accessibility audit', 'Run WCAG 2.1 AA compliance check and fix issues', 'To Do', 2, 1, 3, '2026-02-15 00:00:00');

-- Mobile App Development Project (7 tasks)
INSERT INTO Tasks (Title, Description, Status, Position, ProjectId, AssignedToId, CreatedAt) VALUES
('Setup project structure', 'Initialize React Native project with TypeScript and ESLint', 'Done', 0, 2, 2, '2026-02-15 00:00:00'),
('Design app screens', 'Create Figma mockups for all major app screens', 'Done', 1, 2, 1, '2026-02-15 00:00:00'),
('Implement authentication', 'Build login/signup flow with biometric support', 'In Progress', 0, 2, 3, '2026-02-15 00:00:00'),
('Offline sync logic', 'Implement local storage with server sync queue', 'To Do', 0, 2, 4, '2026-02-15 00:00:00'),
('Push notifications', 'Setup FCM for Android and APNS for iOS notifications', 'To Do', 1, 2, 2, '2026-02-15 00:00:00'),
('App store assets', 'Create screenshots, app icons, and store descriptions', 'To Do', 2, 2, NULL, '2026-02-15 00:00:00'),
('Beta testing', 'Deploy to TestFlight and Play Console for beta users', 'In Review', 0, 2, 5, '2026-02-15 00:00:00');

-- Marketing Campaign Project (6 tasks)
INSERT INTO Tasks (Title, Description, Status, Position, ProjectId, AssignedToId, CreatedAt) VALUES
('Draft press release', 'Write press release announcing Q1 product launch', 'Done', 0, 3, 1, '2026-02-15 00:00:00'),
('Design social media graphics', 'Create templates for Facebook, Twitter, LinkedIn posts', 'In Review', 0, 3, 2, '2026-02-15 00:00:00'),
('Email campaign setup', 'Build email templates and schedule drip campaign', 'In Progress', 0, 3, 4, '2026-02-15 00:00:00'),
('Influencer outreach', 'Identify and contact 20 industry influencers for partnerships', 'To Do', 0, 3, 5, '2026-02-15 00:00:00'),
('Landing page copy', 'Write compelling copy for product launch landing page', 'To Do', 1, 3, NULL, '2026-02-15 00:00:00'),
('Analytics dashboard', 'Setup Mixpanel dashboard for campaign tracking', 'In Progress', 1, 3, 3, '2026-02-15 00:00:00');
```

**Entity Framework Core Model**:
```csharp
public class Task
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Required, MaxLength(50)]
    public string Status { get; set; } = "To Do";  // Enum: To Do, In Progress, In Review, Done

    public int Position { get; set; } = 0;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int? AssignedToId { get; set; }
    public User? AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Comment> Comments { get; } = new List<Comment>();
}
```

---

### 4. Comment

Represents a text message on a task for team collaboration. Comments track authorship for permission enforcement (users can only edit/delete their own comments).

**Table Name**: `Comments`

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| `Id` | `int` | PRIMARY KEY, IDENTITY | Auto-incrementing unique identifier |
| `Content` | `nvarchar(5000)` | NOT NULL | Comment text content |
| `TaskId` | `int` | NOT NULL, FOREIGN KEY → Tasks(Id) ON DELETE CASCADE | Task this comment belongs to |
| `AuthorId` | `int` | NOT NULL, FOREIGN KEY → Users(Id) | User who wrote the comment |
| `CreatedAt` | `timestamp` | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Comment creation time |
| `UpdatedAt` | `timestamp` | NULL | Last edit time (NULL if never edited) |
| `IsDeleted` | `boolean` | NOT NULL, DEFAULT FALSE | Soft delete flag |

**Indexes**:
- `PRIMARY KEY (Id)`
- **COMPOSITE INDEX** `IX_Comments_Task_Created (TaskId, CreatedAt DESC)` - for chronological comment retrieval
- `INDEX IX_Comments_Author (AuthorId)` - for user's comment history

**Relationships**:
- **Many-to-One** with `Task` (comment belongs to one task)
- **Many-to-One** with `User` (comment authored by one user)

**Validation Rules**:
- `Content`: Required, 1-5000 characters, no script tags (XSS prevention)
- `TaskId`: Must reference existing task
- `AuthorId`: Must reference existing user
- **Business Rule**: Users can only edit/delete comments where `AuthorId` matches their user ID

**Soft Delete Strategy**:
- `IsDeleted` flag prevents hard deletion (maintains audit trail)
- Deleted comments are filtered from queries
- UI shows "[Comment deleted]" placeholder instead of content

**Sample Data**: Not seeded (populated during runtime by user interactions)

**Entity Framework Core Model**:
```csharp
public class Comment
{
    public int Id { get; set; }

    [Required, MaxLength(5000)]
    public string Content { get; set; } = null!;

    public int TaskId { get; set; }
    public Task Task { get; set; } = null!;

    public int AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; } = false;
}
```

---

## Fluent API Configuration

**TaskifyDbContext.cs - OnModelCreating**:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // User configuration
    modelBuilder.Entity<User>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Email).IsUnique();

        entity.Property(e => e.Role)
            .HasConversion<string>()
            .HasMaxLength(50);
    });

    // Project configuration
    modelBuilder.Entity<Project>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Name);

        entity.HasMany(e => e.Tasks)
            .WithOne(e => e.Project)
            .HasForeignKey(e => e.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    });

    // Task configuration
    modelBuilder.Entity<Task>(entity =>
    {
        entity.HasKey(e => e.Id);

        // Indexes
        entity.HasIndex(e => e.ProjectId);
        entity.HasIndex(e => e.AssignedToId);
        entity.HasIndex(e => new { e.ProjectId, e.Status, e.Position });
        entity.HasIndex(e => new { e.AssignedToId, e.Status });

        // Relationships
        entity.HasOne(e => e.Project)
            .WithMany(e => e.Tasks)
            .HasForeignKey(e => e.ProjectId)
            .IsRequired();

        entity.HasOne(e => e.AssignedTo)
            .WithMany(e => e.AssignedTasks)
            .HasForeignKey(e => e.AssignedToId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Check constraint for status
        entity.ToTable(t => t.HasCheckConstraint(
            "CK_Task_Status",
            "Status IN ('To Do', 'In Progress', 'In Review', 'Done')"));
    });

    // Comment configuration
    modelBuilder.Entity<Comment>(entity =>
    {
        entity.HasKey(e => e.Id);

        // Indexes
        entity.HasIndex(e => new { e.TaskId, e.CreatedAt });
        entity.HasIndex(e => e.AuthorId);

        // Relationships
        entity.HasOne(e => e.Task)
            .WithMany(e => e.Comments)
            .HasForeignKey(e => e.TaskId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(e => e.Author)
            .WithMany(e => e.AuthoredComments)
            .HasForeignKey(e => e.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Query filter for soft deletes
        entity.HasQueryFilter(e => !e.IsDeleted);
    });

    // Seed data
    SeedData(modelBuilder);
}
```

---

## Database Migrations

**Initial Migration Commands**:
```bash
# Create initial migration
dotnet ef migrations add InitialCreate --project Taskify.ApiService

# Apply migration to database
dotnet ef database update --project Taskify.ApiService

# Generate SQL script for review (optional)
dotnet ef migrations script --project Taskify.ApiService --output migration.sql
```

**Migration Naming Convention**:
- `YYYYMMDD_DescriptiveName` (e.g., `20260215_InitialCreate`)

---

## Query Patterns

### Common Queries

**Get Kanban Board for Project**:
```csharp
var tasks = await _context.Tasks
    .Include(t => t.AssignedTo)
    .Where(t => t.ProjectId == projectId)
    .OrderBy(t => t.Status)
    .ThenBy(t => t.Position)
    .ToListAsync();
```

**Get Tasks Assigned to User**:
```csharp
var userTasks = await _context.Tasks
    .Include(t => t.Project)
    .Include(t => t.Comments)
    .Where(t => t.AssignedToId == userId && t.Status != "Done")
    .OrderBy(t => t.CreatedAt)
    .ToListAsync();
```

**Get Task with Comments**:
```csharp
var task = await _context.Tasks
    .Include(t => t.Project)
    .Include(t => t.AssignedTo)
    .Include(t => t.Comments)
        .ThenInclude(c => c.Author)
    .FirstOrDefaultAsync(t => t.Id == taskId);
```

**Update Task Status (with audit)**:
```csharp
var task = await _context.Tasks.FindAsync(taskId);
if (task != null)
{
    task.Status = newStatus;
    task.UpdatedAt = DateTime.UtcNow;
    await _context.SaveChangesAsync();
}
```

---

## Performance Considerations

1. **Use AsNoTracking() for read-only queries**:
   ```csharp
   var projects = await _context.Projects.AsNoTracking().ToListAsync();
   ```

2. **Project to DTOs for API responses** (reduce over-fetching):
   ```csharp
   var taskDtos = await _context.Tasks
       .Where(t => t.ProjectId == projectId)
       .Select(t => new TaskDto
       {
           Id = t.Id,
           Title = t.Title,
           Status = t.Status,
           AssigneeName = t.AssignedTo != null ? t.AssignedTo.Name : null
       })
       .ToListAsync();
   ```

3. **Use SplitQuery for multiple includes** (avoids cartesian explosion):
   ```csharp
   var tasks = await _context.Tasks
       .Include(t => t.Project)
       .Include(t => t.Comments)
       .AsSplitQuery()
       .ToListAsync();
   ```

---

## Audit Trail

All entities include audit fields:
- `CreatedAt`: Record creation timestamp (UTC)
- `UpdatedAt`: Last modification timestamp (NULL if never updated)

Consider adding in future:
- `CreatedBy` / `UpdatedBy` for user tracking (post-MVP)
- `Version` / `RowVersion` for optimistic concurrency

---

## Summary

This data model supports all MVP requirements:
- **P0 (User Selection)**: User entity with 5 seeded users
- **P1 (Kanban Board)**: Task entity with Status and Position fields
- **P2 (Task Management)**: Task entity with Title, Description, Assignment
- **P3 (Projects)**: Project entity with 3 seeded projects
- **P4 (Comments)**: Comment entity with authorship tracking

All relationships, indexes, and constraints optimize for Kanban board queries while maintaining data integrity and supporting future scaling.
