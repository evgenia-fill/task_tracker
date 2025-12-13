using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Services;
using SMMTracker.WebUI.DTOs;

namespace SMMTracker.WebUI;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager _userManager;
    private readonly ApplicationDbContext _context;

    public AuthController(UserManager userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [HttpPost("api/auth/telegram")]
    public async Task<IActionResult> LoginWithTelegram([FromBody] TelegramLoginData dto)
    {
        var connection = (SqliteConnection)_context.Database.GetDbConnection();
        Console.WriteLine($"[AUTH_CONTROLLER_DEBUG] Сайт использует базу данных: {connection.DataSource}");

        if (dto == null)
        {
            return BadRequest("No dto received");
        }

        var user = new User
        {
            TelegramId = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserName = string.IsNullOrWhiteSpace(dto.Username) ? $"user_{dto.Id}" : dto.Username,
            Hash = Guid.NewGuid().ToString()
        };

        try
        {
            var appUser = await _userManager.FindOrCreateUserAsync(user);
            return Ok(appUser);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH_CONTROLLER_ERROR] Ошибка при сохранении пользователя: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}