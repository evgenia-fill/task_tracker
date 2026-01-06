using Microsoft.AspNetCore.Mvc;
using SMMTracker.Application.Abstractions;
using System.Security.Claims;

namespace SMMTracker.WebUI.Controllers;

[Route("api/check-achievements")]
[ApiController]
public class AchievementsApiController : ControllerBase
{
    private readonly IAchievementService _service;

    public AchievementsApiController(IAchievementService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetNewAchievements()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdString, out int userId))
        {
            var newAchievements = await _service.GetUserNewAchievementsAsync(userId);
            return Ok(newAchievements);
        }
        return Ok(new List<object>());
    }
}