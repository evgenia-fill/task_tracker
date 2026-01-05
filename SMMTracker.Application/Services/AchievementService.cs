using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.Enums;

namespace SMMTracker.Application.Services;

public class AchievementService : IAchievementService
{
    private readonly IApplicationDbContext _context;

    public AchievementService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async System.Threading.Tasks.Task CheckAchievementsAsync(int userId)
    {
        var completedTasksCount = await _context.UserTasks
            .Include(ut => ut.Task)
            .Where(ut => ut.UserId == userId && ut.Task.Status == SMMTracker.Domain.Enums.TaskStatus.Done)
            .CountAsync();
            
        // Логика будет позже
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

        return await _context.Achievements
            .Where(a => achievementIds.Contains(a.Id))
            .ToListAsync();
    }

    public async System.Threading.Tasks.Task<List<Achievement>> GetUserNewAchievementsAsync(int userId)
    {
        return new List<Achievement>();
    }
}