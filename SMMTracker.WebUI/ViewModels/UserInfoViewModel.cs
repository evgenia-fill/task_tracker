namespace SMMTracker.WebUI.ViewModels;

public class UserInfoViewModel
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string TelegramUsername { get; set; } = "";
    public string ProfileDescription { get; set; } = "";
}