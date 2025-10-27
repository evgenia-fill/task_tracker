using Data.models;

namespace BlazorApp1.Services;

public class UserStateService
{
    private BotUser? CurrentUser { get; set; }
    private bool IsLoggedIn => CurrentUser != null;
    public event Action? OnChange;

    public void LogIn(BotUser user)
    {
        CurrentUser = user;
    }

    public void LogOut()
    {
        CurrentUser = null;
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}