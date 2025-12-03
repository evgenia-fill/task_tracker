using System.Collections.Generic;

namespace SMMTracker.Domain.Entities;

public class Team : Entity
{
    public string? Name { get; private set; }
    public string Code { get; set; }
    
    public Calendar? Calendar { get; set; }
    public List<UserTeam>? UserTeams { get; set; }
    public List<Event>? Events { get; set; }

    public Team(string name)
    {
        Name = name;
    }

    private Team()
    {
    }
}