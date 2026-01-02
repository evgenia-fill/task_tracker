using Microsoft.AspNetCore.Mvc;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;

namespace SMMTracker.WebUI.API;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventService _eventService;

    public EventsController(IEventService eventService)
    {
        _eventService = eventService;
    }
    
    // --- ИСПРАВЛЕННЫЙ МЕТОД ---
    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendarEvents([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var events = await _eventService.GetEventsForCalendarAsync(start, end);
    
        // Преобразуем данные в формат, понятный календарю на JS
        // Обычно JS календари (FullCalendar) ждут поля: id, title, start
        return Ok(events.Select(e => new {
            id = e.Id,
            title = e.Name, // Берем из Name
            start = e.Date.ToString("yyyy-MM-ddTHH:mm:ss") // Берем из Date
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
    {
        var eventId = await _eventService.CreateEventAsync(dto);
        return Ok(new { EventId = eventId });
    }

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEventDetails(int eventId)
    {
        var eventDetails = await _eventService.GetEventDetailsAsync(eventId);
        if (eventDetails == null) return NotFound("Event not found");
        return Ok(eventDetails);
    }
}