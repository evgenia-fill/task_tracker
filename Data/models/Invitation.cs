namespace Data.models;

public enum InvitationStatus
{
    Pending,
    Accepted,
    Declined,
}
public class Invitation
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team Team { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public InvitationStatus Status { get; set; }
    
}