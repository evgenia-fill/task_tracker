using Microsoft.AspNetCore.Mvc.RazorPages;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using System.Security.Claims;

namespace SMMTracker.WebUI.Pages;

public class AchievementsModel : PageModel
{
    private readonly IAchievementService _achievementService;

    public List<Achievement> AllAchievements { get; set; } = new();
    public List<SMMTracker.Domain.Entities.Achievement> UserAchievements { get; set; } = new();

    public AchievementsModel(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    public async System.Threading.Tasks.Task OnGetAsync()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // Если ID в клеймах не тот, используй свой способ получения ID, например User.Identity.Name
        if (int.TryParse(userIdString, out int userId))
        {
            AllAchievements = await _achievementService.GetAllAchievementsAsync();
            UserAchievements = await _achievementService.GetUserAchievementsAsync(userId);
        }
    }
}