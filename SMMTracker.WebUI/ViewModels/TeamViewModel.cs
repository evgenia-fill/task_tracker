namespace SMMTracker.WebUI.ViewModels;

public class TeamViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string InvitationCode { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int MemberCount { get; set; }
    public bool IsOwner { get; set; }
}