using System.Collections.Generic;

namespace SMMTracker.Domain.Entities;

public class Calendar : Entity
{
    public int TeamId { get; private set; }
    public Team Team { get; private set; } = null!;
    public List<Event>? Events { get; set; }
    public List<Task>? Tasks { get; set; }

    public Calendar(int teamId)
    {
        TeamId = teamId;
    }

    private Calendar()
    {
    }
}