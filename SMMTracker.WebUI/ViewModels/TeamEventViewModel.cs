namespace SMMTracker.WebUI.ViewModels;

public class TeamEventViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime EventDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
}