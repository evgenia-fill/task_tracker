using SMMTracker.Application.Dtos;

namespace SMMTracker.Infrastructure.Services;

public class UserStateService
{
    public UserDto CurrentUser { get; private set; } 
    
    public event Action OnChange; 

    public void SetUser(UserDto user)
    {
        CurrentUser = user;
        OnChange?.Invoke(); 
    }

    public void LogOut()
    {
        CurrentUser = null;
        OnChange?.Invoke(); 
    }
}