using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SMMTracker.WebUI.ViewModels
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = "";
        
        public string ErrorMessage { get; set; } = "";
        
        public IActionResult OnGet()
        {
            return Page();
        }
        
        public IActionResult OnPostTelegram()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Пожалуйста, введите ваш Telegram username";
                return Page();
            }
            
            // Эмуляция попытки авторизации
            ErrorMessage = "Функция авторизации через Telegram временно недоступна";
            return Page();
        }
        
        // Убрал OnPostTest - теперь используется просто ссылка <a href="/Dashboard">
    }
}