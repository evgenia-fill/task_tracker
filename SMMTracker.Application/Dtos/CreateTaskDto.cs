namespace SMMTracker.Application.Dtos;

public class CreateTaskDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public int CalendarId { get; set; }
}