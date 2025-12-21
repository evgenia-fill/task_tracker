using System.Collections.Generic;

namespace SMMTracker.Domain.Entities;

public class Team : Entity
{
    public string? Name { get; set; }
    public string Code { get; set; }

    public Calendar? Calendar { get; set; }
    public List<UserTeam>? UserTeams { get; set; } = new(); 
    
    public string? Description { get; set; }
    public Team(string name, string code, string? description = null)
    {
        Name = name;
        Code = code;
        Description = description;
    }

    private Team()
    {
    }
}