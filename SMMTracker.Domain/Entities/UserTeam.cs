using SMMTracker.Domain.Enums;

namespace SMMTracker.Domain.Entities;

public class UserTeam : Entity
{
    public int UserId { get; set; }
    public int TeamId { get; set; }
    public TeamRole Role { get; set; }

    public User? User { get; set; }
    public Team? Team { get; set; }
}