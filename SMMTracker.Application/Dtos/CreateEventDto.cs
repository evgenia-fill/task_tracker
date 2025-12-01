namespace SMMTracker.Application.Dtos;

public class CreateEventDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public int CalendarId { get; set; }
}