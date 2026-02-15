using System.Net.Http.Json;

namespace Taskify.Web.Services;

/// <summary>
/// HTTP client service for communicating with Taskify REST APIs.
/// Handles all API requests from Blazor Server to the API Service.
/// </summary>
public class TaskifyApiClient
{
    private readonly HttpClient _httpClient;

    public TaskifyApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // User APIs

    /// <summary>
    /// Gets all users.
    /// </summary>
    public async Task<List<UserDto>?> GetUsersAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<UserDto>>("/api/users");
    }

    /// <summary>
    /// Gets a specific user by ID.
    /// </summary>
    public async Task<UserDto?> GetUserAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<UserDto>($"/api/users/{userId}");
    }

    // Project APIs

    /// <summary>
    /// Gets all projects.
    /// </summary>
    public async Task<List<ProjectDto>?> GetProjectsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<ProjectDto>>("/api/projects");
    }

    /// <summary>
    /// Gets a specific project by ID.
    /// </summary>
    public async Task<ProjectDto?> GetProjectAsync(int projectId)
    {
        return await _httpClient.GetFromJsonAsync<ProjectDto>($"/api/projects/{projectId}");
    }

    // Task APIs

    /// <summary>
    /// Gets all tasks for a specific project.
    /// </summary>
    public async Task<List<TaskDto>?> GetProjectTasksAsync(int projectId)
    {
        return await _httpClient.GetFromJsonAsync<List<TaskDto>>($"/api/projects/{projectId}/tasks");
    }

    /// <summary>
    /// Gets a specific task by ID.
    /// </summary>
    public async Task<TaskDto?> GetTaskAsync(int taskId)
    {
        return await _httpClient.GetFromJsonAsync<TaskDto>($"/api/tasks/{taskId}");
    }

    /// <summary>
    /// Updates a task's status (for drag-and-drop).
    /// </summary>
    public async Task<bool> UpdateTaskStatusAsync(int taskId, string newStatus, int position)
    {
        var response = await _httpClient.PatchAsJsonAsync($"/api/tasks/{taskId}/status", new
        {
            status = newStatus,
            position = position
        });

        return response.IsSuccessStatusCode;
    }

    // Comment APIs

    /// <summary>
    /// Gets all comments for a specific task.
    /// </summary>
    public async Task<List<CommentDto>?> GetTaskCommentsAsync(int taskId)
    {
        return await _httpClient.GetFromJsonAsync<List<CommentDto>>($"/api/tasks/{taskId}/comments");
    }
}

// DTO classes (will be moved to shared library in future)

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}

public class ProjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int TaskCount { get; set; }
}

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public int Position { get; set; }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public int? AssignedToId { get; set; }
    public string? AssigneeName { get; set; }
    public int CommentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public int TaskId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
