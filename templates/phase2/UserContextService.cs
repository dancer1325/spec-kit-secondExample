namespace Taskify.Web.Services;

/// <summary>
/// Manages current user context state for the application.
/// Tracks which user is currently selected and persists across navigation.
/// </summary>
public class UserContextService
{
    private int? _currentUserId;
    private string? _currentUserName;

    /// <summary>
    /// Event raised when the current user changes.
    /// </summary>
    public event EventHandler? UserChanged;

    /// <summary>
    /// Gets the currently selected user ID.
    /// </summary>
    public int? CurrentUserId => _currentUserId;

    /// <summary>
    /// Gets the currently selected user name.
    /// </summary>
    public string? CurrentUserName => _currentUserName;

    /// <summary>
    /// Gets whether a user is currently selected.
    /// </summary>
    public bool HasUser => _currentUserId.HasValue;

    /// <summary>
    /// Sets the current user context.
    /// </summary>
    /// <param name="userId">User ID to set as current.</param>
    /// <param name="userName">User name to set as current.</param>
    public void SetUser(int userId, string userName)
    {
        _currentUserId = userId;
        _currentUserName = userName;
        UserChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Clears the current user context.
    /// </summary>
    public void ClearUser()
    {
        _currentUserId = null;
        _currentUserName = null;
        UserChanged?.Invoke(this, EventArgs.Empty);
    }
}
