namespace Data.models;

public enum EventStatus
{
    Active,
    Archived,
}

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; }
    public EventStatus Status { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public Calendar Calendar { get; set; }
    public ICollection<Task> Tasks { get; set; }
}