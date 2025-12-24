using System;
using System.Collections.Generic;
using SMMTracker.Domain.Enums;

namespace SMMTracker.Domain.Entities;

public class Event : Entity
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public DateTime Date { get; private set; }
    public int CalendarId { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.Now;
    public int CreatedBy { get; private set; }
    public Calendar Calendar { get; private set; } = null!;
    public List<Task> Tasks { get; private set; } = new();
    public int TeamId { get; private set; }
    public Team? Team { get; set; } = null!;

    public Event(string? name, string? description,
        DateTime date, int calendarId, int createdBy, int teamId)
    {
        Name = name;
        Description = description;
        Date = date;
        CalendarId = calendarId;
        CreatedBy = createdBy;
        TeamId = teamId;
    }

    private Event()
    {
    }
}