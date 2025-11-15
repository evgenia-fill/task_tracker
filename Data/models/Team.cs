namespace Data.models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public Calendar Calendar { get; set; }
    public ICollection<UserTeam> UserTeams { get; set; }
    public ICollection<Event> Events { get; set; }
}