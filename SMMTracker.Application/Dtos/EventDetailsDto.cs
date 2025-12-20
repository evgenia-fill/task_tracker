namespace SMMTracker.Application.Dtos;

public class EventDetailsDto
{
    public int Id { get; set; } 
    public int TeamId { get; set; } 
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public List<TaskSummaryDto> Tasks { get; set; } = new();
}
