using SMMTracker.Domain.Enums;
using TaskStatus = SMMTracker.Domain.Enums.TaskStatus;

namespace SMMTracker.Domain.Entities;

public class Task : Entity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime Deadline { get; private set; }
    public TaskStatus Status { get; private set; }
    public int EventId { get; private set; }
    public int? CalendarId { get; private set; }

    public Event Event { get; private set; } = null!;
    public Calendar? Calendar { get; private set; }
    public List<UserTask> UserTasks { get; private set; } = new();

    public Task(string name, string? description, int eventId, int? calendarId)
    {
        Name = name;
        Description = description;
        EventId = eventId;
        CalendarId = calendarId;

        Status = TaskStatus.InProgress;
        CreatedAt = DateTime.UtcNow;
        Deadline = DateTime.UtcNow.AddDays(7);
    }

    private Task()
    {
    }

    public void SetDeadline(DateTime deadline)
    {
        Deadline = deadline;
    }

    public void MoveToReview()
    {
        if (Status != TaskStatus.InProgress) throw new Exception("Cannot move task to Review from this status");
        Status = TaskStatus.Review;
        return;
    }

    public void MoveToDone()
    {
        if (Status != TaskStatus.Review) throw new Exception("Cannot move task to Done from this status");
        Status = TaskStatus.Done;
        return;
    }

    public void ChangeName(string name)
    {
        Name = name;
    }

    public void ChangeDescription(string description)
    {
        Description = description;
    }

    // public void SetPrioritized(bool isPrioritized)
    // {
    //     IsPrioritized = isPrioritized;
    // }
}