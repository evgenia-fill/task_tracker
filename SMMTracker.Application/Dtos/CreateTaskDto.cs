namespace SMMTracker.Application.Dtos;

public class CreateTaskDto
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int EventId { get; set; }
    public int? CalendarId { get; set; }
}