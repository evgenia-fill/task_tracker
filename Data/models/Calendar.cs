using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Data.models;

public class Calendar
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team Team { get; set; }
    
    public ICollection<Event> Events { get; set; }
    public ICollection<Task> Tasks { get; set; }
}