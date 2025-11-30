using SMMTracker.Domain.Enums;
using TaskStatus = SMMTracker.Domain.Enums.TaskStatus;

namespace SMMTracker.Domain.Entities;

public class Task : Entity
{
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime? DeadlineInProgress { get; private set; }
    public DateTime? DeadlineReview { get; private set; }
    public bool IsPrioritized { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int EventId { get; private set; }
    public int CalendarId { get; private set; }

    public Event? Event { get; private set; }
    public Calendar? Calendar { get; private set; }
    public List<UserTask>? UserTasks { get; private set; } = new();

    public Task(string title, string description)
    {
        Title = title;
        Description = description;
        Status = TaskStatus.InProgress;
        CreatedAt = DateTime.UtcNow;
    }

    private Task()
    {
    }

    public void SetDeadlines(DateTime deadlineInProgress, DateTime deadlineReview)
    {
        DeadlineInProgress = deadlineInProgress;
        DeadlineReview = deadlineReview;
    }

    public void MoveToReview()
    {
        if (Status == TaskStatus.InProgress)
            Status = TaskStatus.Review;
        throw new Exception("Cannot move task to review from this status");
    }

    public void MoveToDone()
    {
        if (Status == TaskStatus.Review)
            Status = TaskStatus.Done;
        throw new Exception("Cannot move task to Done from this status");
    }

    public void ChangeTitle(string name)
    {
        Title = name;
    }

    public void ChangeDescription(string description)
    {
        Description = description;
    }

    public void SetPrioritized(bool isPrioritized)
    {
        IsPrioritized = isPrioritized;
    }
}