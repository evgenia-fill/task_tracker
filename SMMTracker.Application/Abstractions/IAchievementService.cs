namespace SMMTracker.Application.Abstractions;

public interface IAchievementService
{
    System.Threading.Tasks.Task CheckAchievementsAsync(int userId);
    System.Threading.Tasks.Task<List<SMMTracker.Domain.Entities.Achievement>> GetAllAchievementsAsync();
    System.Threading.Tasks.Task<List<SMMTracker.Domain.Entities.Achievement>> GetUserAchievementsAsync(int userId);
    System.Threading.Tasks.Task<List<SMMTracker.Domain.Entities.Achievement>> GetUserNewAchievementsAsync(int userId);
}