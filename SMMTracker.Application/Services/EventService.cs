using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.IRepositories;

namespace SMMTracker.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ICalendarRepository _calendarRepository;

    public EventService(IEventRepository eventRepository, ICalendarRepository calendarRepository)
    {
        _eventRepository = eventRepository;
        _calendarRepository = calendarRepository;
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

    public async Task<List<EventSummaryDto>> GetEventsForTeamAsync(int teamId)
    {
        var calendar = await _calendarRepository.GetByTeamIdAsync(teamId);
        if (calendar == null)
        {
            // Временное решение: создаём виртуальный календарь на сервере
            calendar = new Calendar(teamId);
            await _calendarRepository.AddAsync(calendar);
        }

        var events = await _eventRepository.GetEventsForCalendarAsync(calendar.Id);
        return events.Select(e => new EventSummaryDto
        {
            Id = e.Id,
            Name = e.Name,
            Date = e.Date
        }).ToList();
    }
}