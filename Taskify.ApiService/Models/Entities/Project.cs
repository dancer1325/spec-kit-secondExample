using System.ComponentModel.DataAnnotations;

namespace Taskify.ApiService.Models.Entities;

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
