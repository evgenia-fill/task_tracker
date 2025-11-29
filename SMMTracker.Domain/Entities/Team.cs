using System.Collections.Generic;

namespace SMMTracker.Domain.Entities;

public class Team : Entity
{
    public string? Name { get; set; }
    public int CalendarId { get; set; }

    public Calendar? Calendar { get; set; }
    public List<UserTeam>? UserTeams { get; set; }
    public List<Event>? Events { get; set; }
}