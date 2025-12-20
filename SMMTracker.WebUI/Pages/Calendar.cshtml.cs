using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SMMTracker.Application.Abstractions; 
using System.Security.Claims;

namespace SMMTracker.WebUI.Pages
{
    [Authorize]
    public class CalendarModel : PageModel
    {
        private readonly ICalendarService _calendarService;

        [BindProperty] public int CurrentCalendarId { get; set; }
        public int TeamId { get; set; }

        public CalendarModel(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var info = await _calendarService.GetCalendarInfoByUserIdAsync(userId);

            CurrentCalendarId = info.CalendarId;
            TeamId = info.TeamId;
            
            return Page();
        }
    }
}