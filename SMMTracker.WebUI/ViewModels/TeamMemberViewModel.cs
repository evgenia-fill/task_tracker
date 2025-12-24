namespace SMMTracker.WebUI.ViewModels;

public class TeamMemberViewModel
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string TelegramUsername { get; set; } = "";
    public string Role { get; set; } = "";
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}