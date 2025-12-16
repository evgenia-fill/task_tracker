using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SMMTracker.Application.Services;
using System.Security.Claims; // Нужен для User.FindFirstValue

namespace SMMTracker.WebUI.Pages
{
    // Убедитесь, что эта страница доступна только авторизованным пользователям
    [Authorize] 
    public class CalendarModel : PageModel
    {
        private readonly CalendarService _calendarService;

        // Это свойство будет доступно в Razor Pages через @Model.CurrentCalendarId
        [BindProperty]
        public int CurrentCalendarId { get; set; }

        public CalendarModel(CalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // 1. Получаем ID текущего авторизованного пользователя
            // ClaimTypes.NameIdentifier - это стандартное поле для User ID в ASP.NET Identity
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                // Если по какой-то причине ID пользователя не найден, 
                // перенаправляем на страницу входа (или возвращаем Forbidden)
                return RedirectToPage("/Account/Login");
            }

            // 2. Ищем календарь этого пользователя через сервис
            // Предполагается, что _calendarService.GetCalendarIdByUserIdAsync(userId)
            // возвращает ID календаря (int) или 0, если календарь не найден.
            var calendarId = await _calendarService.GetCalendarIdByUserIdAsync(userId);

            // 3. Сохраняем результат в свойстве модели
            CurrentCalendarId = calendarId;

            // Если CurrentCalendarId == 0, фронтенд (Calendar.cshtml) покажет сообщение, 
            // что календарь не найден, вместо того чтобы пытаться загрузить данные.

            return Page();
        }
    }
}