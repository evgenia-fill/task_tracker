using Microsoft.AspNetCore.Mvc;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;

namespace SMMTracker.WebUI.API;

[ApiController]
[Route("api/[controller]")]
public class CalendarsController : ControllerBase
{
    private readonly ICalendarService _calendarService;
    private readonly IEventService _eventService;

    public CalendarsController(ICalendarService calendarService, IEventService eventService)
    {
        _calendarService = calendarService;
        _eventService = eventService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCalendar([FromBody] CreateCalendarDto dto)
    {
        var calendarId = await _calendarService.CreateCalendarAsync(dto);
        return Ok(new { CalendarId = calendarId });
    }

    [HttpGet("{calendarId}/events")]
    public async Task<IActionResult> GetCalendarEvents(int calendarId, [FromQuery] int month, [FromQuery] int year)
    {
        var monthEvents = await _eventService.GetEventsForMonthAsync(calendarId, month, year);
        return Ok(monthEvents);
    }
}