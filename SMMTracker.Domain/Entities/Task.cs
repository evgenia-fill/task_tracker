
using SMMTracker.Domain.Enums;
using TaskStatus = SMMTracker.Domain.Enums.TaskStatus;

namespace SMMTracker.Domain.Entities;

public class Task : Entity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime Deadline { get; set; }
    public TaskStatus Status { get; set; }
    public int EventId { get; set; }
    public int CalendarId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Event? Event { get; set; }
    public Calendar? Calendar { get; set; }
    public List<UserTask>? UserTasks { get; set; }
}