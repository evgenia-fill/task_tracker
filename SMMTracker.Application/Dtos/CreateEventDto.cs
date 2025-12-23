namespace SMMTracker.Application.Dtos;

public class CreateEventDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public int TeamId { get; set; }
    public int CreatedBy { get; set; }
}