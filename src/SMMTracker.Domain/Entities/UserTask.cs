using SMMTracker.Domain.Enums;

namespace SMMTracker.Domain.Entities;

public class UserTask : Entity
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public UserTaskRole Role { get; set; }

    public User User { get; set; }
    public Task Task { get; set; }
}