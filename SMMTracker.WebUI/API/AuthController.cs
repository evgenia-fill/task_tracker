using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Entities;
using SMMTracker.Infrastructure.Data.DataContext;

namespace SMMTracker.WebUI.API;

[ApiController]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(IUserService userService, ApplicationDbContext context, IConfiguration configuration)
    {
        _userService = userService;
        _context = context;
        _configuration = configuration;
    }

    [HttpGet("/api/auth/telegram-callback")]
    public async Task<IActionResult> TelegramCallback(
        [FromQuery] long id,
        [FromQuery] string first_name,
        [FromQuery] string? last_name,
        [FromQuery] string? username,
        [FromQuery] string? photo_url,
        [FromQuery] long auth_date,
        [FromQuery] string hash)
    {
        try
        {
            var connection = (SqliteConnection)_context.Database.GetDbConnection();
            Console.WriteLine($"[TELEGRAM_AUTH_DEBUG] База данных: {connection.DataSource}");
            Console.WriteLine($"[TELEGRAM_AUTH_DEBUG] Данные пользователя: {id}, {first_name}, {username}");

            var botToken = _configuration["Telegram:BotToken"];
            if (string.IsNullOrEmpty(botToken))
            {
                Console.WriteLine("[TELEGRAM_AUTH_ERROR] Токен бота не найден в конфигурации");
                return Redirect("/Login?error=auth_failed");
            }

            var isValid = ValidateTelegramData(botToken, id, first_name, last_name, 
                username, photo_url, auth_date, hash);
            
            if (!isValid)
            {
                Console.WriteLine($"[TELEGRAM_AUTH_ERROR] Неверная подпись данных");
                return Redirect("/Login?error=invalid_data");
            }

            var authDateTime = DateTimeOffset.FromUnixTimeSeconds(auth_date);
            if (DateTimeOffset.UtcNow - authDateTime > TimeSpan.FromMinutes(1))
            {
                Console.WriteLine($"[TELEGRAM_AUTH_ERROR] Данные устарели: {authDateTime}");
                return Redirect("/Login?error=timeout");
            }

            var user = new User
            {
                TelegramId = id,
                FirstName = first_name,
                LastName = last_name ?? "",
                UserName = username ?? $"user_{id}",
                Hash = Guid.NewGuid().ToString()
            };

            var appUser = await _userService.FindOrCreateUserAsync(user);
            Console.WriteLine($"[TELEGRAM_AUTH_SUCCESS] Пользователь авторизован: {appUser.Id}, {appUser.UserName}");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new Claim(ClaimTypes.Name, appUser.UserName),
                new Claim("TelegramId", appUser.TelegramId.ToString()),
                new Claim("FirstName", appUser.FirstName),
                new Claim("LastName", appUser.LastName ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect("/Dashboard");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TELEGRAM_AUTH_ERROR] Ошибка: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[TELEGRAM_AUTH_ERROR] Внутренняя ошибка: {ex.InnerException.Message}");
            }
            return Redirect("/Login?error=auth_failed");
        }
    }

    private bool ValidateTelegramData(string botToken, long id, string firstName, 
    string? lastName, string? username, string? photoUrl, long authDate, string hash)
{
    try
    {
        Console.WriteLine($"[VALIDATE_DEBUG] Токен: {botToken[..15]}...");
        
        var dataCheckDict = new Dictionary<string, string>
        {
            ["auth_date"] = authDate.ToString(),
            ["first_name"] = firstName,
            ["id"] = id.ToString()
        };
        
        if (!string.IsNullOrEmpty(lastName))
            dataCheckDict["last_name"] = lastName;
            
        if (!string.IsNullOrEmpty(photoUrl))
            dataCheckDict["photo_url"] = photoUrl;
            
        if (!string.IsNullOrEmpty(username))
            dataCheckDict["username"] = username;
        
        var sortedKeys = dataCheckDict.Keys.OrderBy(k => k).ToList();
        
        var dataCheckArray = new List<string>();
        foreach (var key in sortedKeys)
            dataCheckArray.Add($"{key}={dataCheckDict[key]}");
        
        var dataCheckString = string.Join("\n", dataCheckArray);
        
        Console.WriteLine($"[VALIDATE_DEBUG] Data string для хеша:");
        Console.WriteLine($"\"{dataCheckString.Replace("\n", "\\n")}\"");
        Console.WriteLine($"[VALIDATE_DEBUG] Ожидаемый хеш: {hash}");
        
        byte[] secretKey;
        using (var sha256 = SHA256.Create())
        {
            secretKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(botToken));
        }
        
        using var hmac = new HMACSHA256(secretKey);
        var computedHashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));
        var computedHashString = BitConverter.ToString(computedHashBytes)
            .Replace("-", "")
            .ToLower();
        
        Console.WriteLine($"[VALIDATE_DEBUG] Вычисленный хеш: {computedHashString}");
        Console.WriteLine($"[VALIDATE_DEBUG] Совпадают: {computedHashString == hash.ToLower()}");
        
        Console.WriteLine($"[VALIDATE_DEBUG] Альтернативный расчет с raw токеном...");
        using var hmac2 = new HMACSHA256(Encoding.UTF8.GetBytes(botToken));
        var computedHash2 = BitConverter.ToString(hmac2.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString)))
            .Replace("-", "")
            .ToLower();
        Console.WriteLine($"[VALIDATE_DEBUG] С raw токеном: {computedHash2}");
        
        return computedHashString == hash.ToLower() || computedHash2 == hash.ToLower();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[VALIDATE_ERROR] {ex.Message}");
        return false;
    }
}

    [HttpPost("api/auth/telegram")]
    public async Task<IActionResult> LoginWithTelegram([FromBody] TelegramLoginDto? dto)
    {
        var connection = (SqliteConnection)_context.Database.GetDbConnection();
        Console.WriteLine($"[AUTH_CONTROLLER_DEBUG] Сайт использует базу данных: {connection.DataSource}");

        if (dto == null)
            return BadRequest("No dto received");

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
            var appUser = await _userService.FindOrCreateUserAsync(user);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
                new(ClaimTypes.Name, appUser.UserName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok(appUser);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AUTH_CONTROLLER_ERROR] Ошибка при сохранении пользователя: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}