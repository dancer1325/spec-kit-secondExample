using System.ComponentModel.DataAnnotations;

namespace Taskify.ApiService.Models.Entities;

/// <summary>
/// Represents a work item with title, description, status, and assignment.
/// Tasks belong to a project and can be assigned to a user.
/// </summary>
public class Task
{
    /// <summary>
    /// Unique identifier for the task.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Task title (required, 1-200 characters).
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = null!;

    /// <summary>
    /// Detailed task description (optional, max 2000 characters).
    /// </summary>
    [MaxLength(2000)]
    public string? Description { get; set; }

    /// <summary>
    /// Current status: "To Do", "In Progress", "In Review", or "Done".
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "To Do";

    /// <summary>
    /// Position within status column (for ordering).
    /// </summary>
    public int Position { get; set; } = 0;

    /// <summary>
    /// Foreign key to Project.
    /// </summary>
    public int ProjectId { get; set; }

    /// <summary>
    /// Navigation property to Project.
    /// </summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// Foreign key to User (nullable - task may be unassigned).
    /// </summary>
    public int? AssignedToId { get; set; }

    /// <summary>
    /// Navigation property to assigned User.
    /// </summary>
    public User? AssignedTo { get; set; }

    /// <summary>
    /// Record creation timestamp (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last modification timestamp (UTC).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties

    /// <summary>
    /// Comments on this task.
    /// </summary>
    public ICollection<Comment> Comments { get; } = new List<Comment>();
}
