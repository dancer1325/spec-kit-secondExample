using System.ComponentModel.DataAnnotations;

namespace Taskify.ApiService.Models.Entities;

/// <summary>
/// Represents a work initiative containing tasks.
/// Three sample projects are seeded for MVP.
/// </summary>
public class Project
{
    /// <summary>
    /// Unique identifier for the project.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Project name.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Project description.
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Description { get; set; } = null!;

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
    /// Tasks belonging to this project.
    /// </summary>
    public ICollection<Task> Tasks { get; } = new List<Task>();
}
