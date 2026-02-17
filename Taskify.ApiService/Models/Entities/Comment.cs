using System.ComponentModel.DataAnnotations;

namespace Taskify.ApiService.Models.Entities;

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
