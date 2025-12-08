using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;

namespace SMMTracker.WebUI.API;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TeamsController : ControllerBase
{
    private readonly TeamService _teamService;

    public TeamsController(TeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamDto dto)
    {
        var creatorIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(creatorIdString) || !int.TryParse(creatorIdString, out var creatorId))
        {
            return Unauthorized("Invalid user identifier");
        }

        var teamId = await _teamService.CreateTeamAsync(dto, creatorId);
        return Ok(new { TeamId = teamId });
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinTeam([FromBody] JoinTeamDto dto)
    {
        var ok = await _teamService.JoinTeamAsync(dto);
        return ok ? Ok() : BadRequest("Invalid team code");
    }

    [HttpPost("{teamId}/remove-user")]
    public async Task<IActionResult> RemoveUser(int teamId, [FromBody] RemoveUserDto dto)
    {
        try
        {
            await _teamService.RemoveUserFromTeamAsync(teamId, dto.UserId, dto.AdminId);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{teamId}/leave")]
    public async Task<IActionResult> LeaveTeam(int teamId, [FromBody] LeaveTeamDto dto)
    {
        try
        {
            var result = await _teamService.LeaveTeamAsync(teamId, dto.UserId);
            if (!result)
                return BadRequest("User is not in the team");
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}