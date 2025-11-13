namespace Data.models;

public enum TeamRole
{
    Member,
    Admin,
    Owner
}

public class UserTeam
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int TeamId { get; set; }
    public Team Team { get; set; }
    public TeamRole Role { get; set; }
}