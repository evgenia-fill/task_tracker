using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace SMMTracker.WebUI.ViewModels;

public class LoginModel : PageModel
{
    public string BotName { get; set; } = "";
    public string CallbackUrl { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
    public bool IsAuthenticated { get; set; }

    private readonly IConfiguration _configuration;

    public LoginModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult OnGet(string? error = null)
    {
        IsAuthenticated = User.Identity?.IsAuthenticated == true &&
                          User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier);

        if (IsAuthenticated)
        {
            Console.WriteLine($"[LOGIN_DEBUG] Пользователь авторизован: {User.Identity?.Name}");
            Console.WriteLine(
                $"[LOGIN_DEBUG] Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}:{c.Value}"))}");
        }
        else
        {
            Console.WriteLine($"[LOGIN_DEBUG] Пользователь НЕ авторизован");
        }

        if (!string.IsNullOrEmpty(error))
        {
            ErrorMessage = error switch
            {
                "auth_failed" => "Ошибка авторизации. Попробуйте снова.",
                "invalid_data" => "Неверные данные авторизации.",
                "timeout" => "Время авторизации истекло. Попробуйте снова.",
                _ => "Произошла ошибка при авторизации."
            };
        }

        BotName = _configuration["Telegram:BotName"] ?? "SmmTrackerTestBot_bot";

        CallbackUrl = "https://smmtracker.ru//api/auth/telegram-callback";

        return Page();
    }

    public async Task<IActionResult> OnPostForceLogout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToPage("/Login");
    }
}