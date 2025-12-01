using SMMTracker.Domain.Entities;

namespace SMMTracker.WebUI.Services;

public class UserStateService
{
    private User? _currentUser;

    public User? CurrentUser => _currentUser;

    public void SetUser(User user)
    {
        _currentUser = user;
        NotifyStateChanged();
    }

    public void LogOut()
    {
        _currentUser = null;
        NotifyStateChanged();
    }

    public event Action? OnStateChange;

    private void NotifyStateChanged() => OnStateChange?.Invoke();
}