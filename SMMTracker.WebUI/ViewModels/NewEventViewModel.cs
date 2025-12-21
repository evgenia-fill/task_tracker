namespace SMMTracker.WebUI.ViewModels;

public class NewEventViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime EventDate { get; set; } = DateTime.Now.AddDays(1);
}
