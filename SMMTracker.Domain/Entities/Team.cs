using System.Collections.Generic;

namespace SMMTracker.Domain.Entities;

public class Team : Entity
{
    public string? Name { get; private set; }
    public string Code { get; set; }

    public Calendar? Calendar { get; set; }
    public List<UserTeam>? UserTeams { get; set; } = new(); 

    public Team(string name, string code)
    {
        Name = name;
        Code = code;
    }

    private Team()
    {
    }
}