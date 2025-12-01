using Microsoft.AspNetCore.Http.HttpResults;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SMMTracker.Application.Services;

namespace BlazorApp1.API;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly EventService _eventService;

    public EventsController(EventService eventService)
    {
        _eventService = eventService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
    {
        var eventId = await _eventService.CreateEventAsync(dto);
        return Ok(new { EventId = eventId });
    }
}