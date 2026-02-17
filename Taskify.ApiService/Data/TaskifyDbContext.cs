using Microsoft.EntityFrameworkCore;
using Taskify.ApiService.Models.Entities;

namespace Taskify.ApiService.Data;

public class TaskifyDbContext : DbContext
{
    public TaskifyDbContext(DbContextOptions<TaskifyDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Models.Entities.Task> Tasks => Set<Models.Entities.Task>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
        modelBuilder.Entity<Models.Entities.Task>(entity =>
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
                "\"Status\" IN ('To Do', 'In Progress', 'In Review', 'Done')"));
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

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed 5 predefined users
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Sarah Chen", Email = "sarah.chen@taskify.com", Role = "ProductManager", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = 2, Name = "Marcus Rodriguez", Email = "marcus.rodriguez@taskify.com", Role = "Engineer", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = 3, Name = "Aisha Patel", Email = "aisha.patel@taskify.com", Role = "Engineer", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = 4, Name = "James Kim", Email = "james.kim@taskify.com", Role = "Engineer", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
            new User { Id = 5, Name = "Emma Thompson", Email = "emma.thompson@taskify.com", Role = "Engineer", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed 3 sample projects
        modelBuilder.Entity<Project>().HasData(
            new Project { Id = 1, Name = "Website Redesign", Description = "Modernize company website UI/UX with responsive design and improved accessibility", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Project { Id = 2, Name = "Mobile App Development", Description = "Build iOS and Android apps for customer engagement with offline sync capabilities", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Project { Id = 3, Name = "Marketing Campaign", Description = "Q1 product launch campaign including social media, email marketing, and PR outreach", CreatedAt = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc) }
        );

        // Seed 20 sample tasks distributed across projects and columns
        var baseDate = new DateTime(2026, 2, 15, 0, 0, 0, DateTimeKind.Utc);

        // Website Redesign Project (7 tasks)
        modelBuilder.Entity<Models.Entities.Task>().HasData(
            new Models.Entities.Task { Id = 1, Title = "Design new homepage", Description = "Create mockups for homepage with modern design and clear CTAs", Status = "In Progress", Position = 0, ProjectId = 1, AssignedToId = 2, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 2, Title = "Implement navigation menu", Description = "Build responsive navigation with dropdown menus and mobile hamburger", Status = "To Do", Position = 0, ProjectId = 1, AssignedToId = 3, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 3, Title = "Add contact form", Description = "Create contact form with validation and email notifications", Status = "To Do", Position = 1, ProjectId = 1, AssignedToId = null, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 4, Title = "Optimize images", Description = "Compress all images and implement lazy loading", Status = "In Review", Position = 0, ProjectId = 1, AssignedToId = 4, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 5, Title = "Setup analytics", Description = "Integrate Google Analytics and setup conversion tracking", Status = "Done", Position = 0, ProjectId = 1, AssignedToId = 2, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 6, Title = "Write documentation", Description = "Document CMS usage and deployment process", Status = "In Progress", Position = 1, ProjectId = 1, AssignedToId = 5, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 7, Title = "Accessibility audit", Description = "Run WCAG 2.1 AA compliance check and fix issues", Status = "To Do", Position = 2, ProjectId = 1, AssignedToId = 3, CreatedAt = baseDate }
        );

        // Mobile App Development Project (7 tasks)
        modelBuilder.Entity<Task>().HasData(
            new Models.Entities.Task { Id = 8, Title = "Setup project structure", Description = "Initialize React Native project with TypeScript and ESLint", Status = "Done", Position = 0, ProjectId = 2, AssignedToId = 2, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 9, Title = "Design app screens", Description = "Create Figma mockups for all major app screens", Status = "Done", Position = 1, ProjectId = 2, AssignedToId = 1, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 10, Title = "Implement authentication", Description = "Build login/signup flow with biometric support", Status = "In Progress", Position = 0, ProjectId = 2, AssignedToId = 3, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 11, Title = "Offline sync logic", Description = "Implement local storage with server sync queue", Status = "To Do", Position = 0, ProjectId = 2, AssignedToId = 4, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 12, Title = "Push notifications", Description = "Setup FCM for Android and APNS for iOS notifications", Status = "To Do", Position = 1, ProjectId = 2, AssignedToId = 2, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 13, Title = "App store assets", Description = "Create screenshots, app icons, and store descriptions", Status = "To Do", Position = 2, ProjectId = 2, AssignedToId = null, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 14, Title = "Beta testing", Description = "Deploy to TestFlight and Play Console for beta users", Status = "In Review", Position = 0, ProjectId = 2, AssignedToId = 5, CreatedAt = baseDate }
        );

        // Marketing Campaign Project (6 tasks)
        modelBuilder.Entity<Task>().HasData(
            new Models.Entities.Task { Id = 15, Title = "Draft press release", Description = "Write press release announcing Q1 product launch", Status = "Done", Position = 0, ProjectId = 3, AssignedToId = 1, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 16, Title = "Design social media graphics", Description = "Create templates for Facebook, Twitter, LinkedIn posts", Status = "In Review", Position = 0, ProjectId = 3, AssignedToId = 2, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 17, Title = "Email campaign setup", Description = "Build email templates and schedule drip campaign", Status = "In Progress", Position = 0, ProjectId = 3, AssignedToId = 4, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 18, Title = "Influencer outreach", Description = "Identify and contact 20 industry influencers for partnerships", Status = "To Do", Position = 0, ProjectId = 3, AssignedToId = 5, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 19, Title = "Landing page copy", Description = "Write compelling copy for product launch landing page", Status = "To Do", Position = 1, ProjectId = 3, AssignedToId = null, CreatedAt = baseDate },
            new Models.Entities.Task { Id = 20, Title = "Analytics dashboard", Description = "Setup Mixpanel dashboard for campaign tracking", Status = "In Progress", Position = 1, ProjectId = 3, AssignedToId = 3, CreatedAt = baseDate }
        );
    }
}
