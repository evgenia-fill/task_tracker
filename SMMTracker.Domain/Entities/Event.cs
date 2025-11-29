using System;
using System.Collections.Generic;
using SMMTracker.Domain.Enums;

namespace SMMTracker.Domain.Entities;

public class Event : Entity
{
    public string? Name { get; set; }
    public EventStatus Status { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public int CalendarId { get; set; }

    public Calendar? Calendar { get; set; }
    public List<Task>? Tasks { get; set; } 
}