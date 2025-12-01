using Microsoft.AspNetCore.Mvc;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;

namespace SMMTracker.WebUI.API;

[ApiController]
[Route("api/[controller]")] 
public class CalendarsController : ControllerBase
{
    private readonly CalendarService _calendarService;

    public CalendarsController(CalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCalendar([FromBody] CreateCalendarDto dto)
    {
        var calendarId = await _calendarService.CreateCalendarAsync(dto);
        return Ok(new { CalendarId = calendarId });
    }
}