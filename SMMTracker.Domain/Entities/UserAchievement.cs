namespace SMMTracker.Domain.Entities;

public class UserAchievement : Entity
{
    public int UserId { get; set; }
    public User User { get; set; }

    public int AchievementId { get; set; }
    public Achievement Achievement { get; set; }

    public DateTime DateReceived { get; set; } = DateTime.UtcNow;
    public bool IsViewed { get; set; } = false; // Чтобы показать попап один раз
}