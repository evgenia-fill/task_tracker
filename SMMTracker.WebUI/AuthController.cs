using Data;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Domain.Entities;
using SMMTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.DTOs;

namespace BlazorApp1;

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
    public async Task<IActionResult> LoginWithTelegram([FromBody] TelegramLoginData data)
    {
        var connection = (SqliteConnection)_context.Database.GetDbConnection();
        Console.WriteLine($"[AUTH_CONTROLLER_DEBUG] Сайт использует базу данных: {connection.DataSource}");

        if (data == null)
        {
            return BadRequest("No data received");
        }

        var user = new User
        {
            TelegramId = data.Id,
            FirstName = data.FirstName,
            LastName = data.LastName,
            UserName = string.IsNullOrWhiteSpace(data.Username) ? $"user_{data.Id}" : data.Username,
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