using System.ComponentModel.DataAnnotations;

namespace Taskify.ApiService.Models.Entities;

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
