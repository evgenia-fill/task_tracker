using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;

namespace SMMTracker.Application.Services;

public class AchievementService : IAchievementService
{
    private readonly IApplicationDbContext _context;

    public AchievementService(IApplicationDbContext context)
    {
        _context = context;
    }

    // ТЕПЕРЬ ВОЗВРАЩАЕТ СПИСОК ПОЛУЧЕННЫХ АЧИВОК
    public async System.Threading.Tasks.Task<List<Achievement>> CheckAchievementsAsync(int userId)
    {
        var teamsCount = await _context.UserTeams.Where(ut => ut.UserId == userId).CountAsync();
        var allAchievements = await _context.Achievements.ToListAsync();
        var userAchievementIds = await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync();

        var newAchievements = allAchievements
            .Where(a => !userAchievementIds.Contains(a.Id) && teamsCount >= a.TasksThreshold)
            .ToList();

        if (newAchievements.Any())
        {
            foreach (var achievement in newAchievements)
            {
                _context.UserAchievements.Add(new UserAchievement
                {
                    UserId = userId,
                    AchievementId = achievement.Id,
                    DateReceived = DateTime.Now
                });
            }
            await _context.SaveChangesAsync();
        }

        return newAchievements; // Возвращаем список для анимации
    }

    public async System.Threading.Tasks.Task<List<SMMTracker.Domain.Entities.Achievement>> GetAllAchievementsAsync()
    {
        return await _context.Achievements.ToListAsync();
    }

    public async System.Threading.Tasks.Task<List<SMMTracker.Domain.Entities.Achievement>> GetUserAchievementsAsync(int userId)
    {
        var achievementIds = await _context.UserAchievements
            .Where(ua => ua.UserId == userId)
            .Select(ua => ua.AchievementId)
            .ToListAsync();

        return await _context.Achievements.Where(a => achievementIds.Contains(a.Id)).ToListAsync();
    }

    public async System.Threading.Tasks.Task<List<SMMTracker.Domain.Entities.Achievement>> GetUserNewAchievementsAsync(int userId)
    {
        return new List<SMMTracker.Domain.Entities.Achievement>();
    }
}