using SMMTracker.Domain.Enums;

namespace SMMTracker.Domain.Entities;

public class Invitation : Entity
{
    // в бд
    public int TeamId { get; set; }
    public int UserId { get; set; }
    public InvitationStatus Status { get; set; }
    
    // связи (в бд не идут)
    public Team Team { get; set; }
    public User User { get; set; }
}