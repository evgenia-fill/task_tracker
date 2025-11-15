namespace Data.models;

public class User
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhotoUrl { get; set; }
    public string? Bio { get; set; }
    public string Hash { get; set; }
    
    public ICollection<Invitation> Invitations { get; set; }
    public ICollection<UserTeam> Teams { get; set; }
    public ICollection<UserTask> Tasks { get; set; }
}