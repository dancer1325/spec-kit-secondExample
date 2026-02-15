using System.ComponentModel.DataAnnotations;

namespace Taskify.ApiService.Models.Entities;

/// <summary>
/// Represents a text message on a task for team collaboration.
/// Comments track authorship for permission enforcement (users can only edit/delete their own comments).
/// </summary>
public class Comment
{
    /// <summary>
    /// Unique identifier for the comment.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Comment text content (required, 1-5000 characters).
    /// </summary>
    [Required]
    [MaxLength(5000)]
    public string Content { get; set; } = null!;

    /// <summary>
    /// Foreign key to Task.
    /// </summary>
    public int TaskId { get; set; }

    /// <summary>
    /// Navigation property to Task.
    /// </summary>
    public Task Task { get; set; } = null!;

    /// <summary>
    /// Foreign key to User (author).
    /// </summary>
    public int AuthorId { get; set; }

    /// <summary>
    /// Navigation property to author User.
    /// </summary>
    public User Author { get; set; } = null!;

    /// <summary>
    /// Comment creation timestamp (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Last edit timestamp (UTC, null if never edited).
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Soft delete flag (maintains audit trail).
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}
