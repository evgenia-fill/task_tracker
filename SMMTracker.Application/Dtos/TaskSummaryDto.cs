namespace SMMTracker.Application.Dtos;

public class TaskSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public TaskStatus Status { get; set; }
}