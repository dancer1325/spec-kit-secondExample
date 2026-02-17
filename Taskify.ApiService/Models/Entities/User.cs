using System.ComponentModel.DataAnnotations;

namespace Taskify.ApiService.Models.Entities;

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
