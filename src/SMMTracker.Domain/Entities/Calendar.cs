using System.Collections.Generic;

namespace SMMTracker.Domain.Entities;

public class Calendar : Entity
{
    public Team Team { get; set; }
    public List<Event> Events { get; set; } 
    public List<Task> Tasks { get; set; } 
}