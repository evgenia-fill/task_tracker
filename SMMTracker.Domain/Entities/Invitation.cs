using SMMTracker.Domain.Enums;

namespace SMMTracker.Domain.Entities;

public class Invitation : Entity
{
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public InvitationStatus Status { get; set; }
    
    public Team? Team { get; set; }
    public User? User { get; set; }
}