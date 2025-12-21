using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;

namespace SMMTracker.WebUI.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITeamService _teamService;

    public UsersController(IUserService userService, ITeamService teamService)
    {
        _userService = userService;
        _teamService = teamService;
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUserProfile(int userId)
    {
        try
        {
            var profile = await _userService.GetUserProfileAsync(userId);
            return Ok(profile);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UserProfileDto request)
    {
        try
        {
            var userId = GetUserId();
            await _userService.UpdateUserProfileAsync(
                userId,
                request);

            return Ok(new { Message = "Профиль успешно обновлен" });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { e.Message });
        }
    }

    [HttpGet("{userId}/teams")]
    public async Task<IActionResult> GetTeamsForUserAsync(int userId)
    {
        var teams = await _teamService.GetTeamsForUserAsync(userId);
        return Ok(teams);
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Пользователь не авторизован");
        return userId;
    }
}