using TaskStatus = SMMTracker.Domain.Enums.TaskStatus;

namespace SMMTracker.Application.Dtos;

public class TaskSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; } 
    public string AssignedUserName { get; set; }
}