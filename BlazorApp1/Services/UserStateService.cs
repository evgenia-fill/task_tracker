using Data.models;

namespace BlazorApp1.Services;

public class UserStateService
{
    private BotUser? _currentUser;

    public BotUser? CurrentUser => _currentUser;

    public void SetUser(BotUser user)
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