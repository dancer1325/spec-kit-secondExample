using System.ComponentModel.DataAnnotations;

namespace Taskify.ApiService.Models.Entities;

/// <summary>
/// Represents a team member with predefined roles.
/// Five users are seeded for MVP (1 PM, 4 Engineers).
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's full name.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// Email address (future auth use).
    /// </summary>
    [Required]
    [MaxLength(254)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    /// <summary>
    /// User role: "ProductManager" or "Engineer".
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = null!;

    /// <summary>
    /// Record creation timestamp (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties

    /// <summary>
    /// Tasks assigned to this user.
    /// </summary>
    public ICollection<Task> AssignedTasks { get; } = new List<Task>();

    /// <summary>
    /// Comments authored by this user.
    /// </summary>
    public ICollection<Comment> AuthoredComments { get; } = new List<Comment>();
}
