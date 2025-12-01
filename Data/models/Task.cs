namespace Data.models;

public enum TaskStatus
{
    inProgress,
    Review,
    Completed,
}

public class Task
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Deadline { get; set; }
    public TaskStatus Status { get; set; }
    
    public Event Event { get; set; }
    
}