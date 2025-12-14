using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.IRepositories;

namespace SMMTracker.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    public EventService(IEventRepository eventRepository)
    {
        _eventRepository = eventRepository;
    }

    public async Task<int> CreateEventAsync(CreateEventDto dto,
        CancellationToken cancellationToken = default)
    {
        var eventAs = new Event(
            dto.Name,
            dto.Description,
            dto.Date,
            dto.CalendarId
        );
        await _eventRepository.AddAsync(eventAs);
        return eventAs.Id;
    }

    public async Task<List<EventSummaryDto>> GetEventsForMonthAsync(int calendarId, int month, int year,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetEventsForMonthAsync(calendarId, month, year);
        
        return events
            .Select(e => new EventSummaryDto
            {
                Id = e.Id,
                Name = e.Name,
                Date = e.Date
            })
            .ToList();
    }

    public async Task<EventDetailsDto?> GetEventDetailsAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        
        if (eventEntity == null)
            return null;
            
        return new EventDetailsDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Date = eventEntity.Date,
            Tasks = eventEntity.Tasks
                .Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Status = (TaskStatus)t.Status
                }).ToList()
        };
    }
}