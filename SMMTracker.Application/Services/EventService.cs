using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos; // Убедитесь, что EventSummaryDto здесь
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

    // ... (ваши существующие методы CreateEventAsync, GetEventsForMonthAsync и т.д.) ...
    
    public async Task<int> CreateEventAsync(CreateEventDto dto, CancellationToken cancellationToken = default)
    {
        var calendar = await _calendarRepository.GetByTeamIdAsync(dto.TeamId, cancellationToken);
        if (calendar == null)
            throw new InvalidOperationException($"Не найден календарь для команды с Id={dto.TeamId}");

        var eventAs = new Event(
            dto.Name,
            dto.Description,
            dto.Date,
            calendar.Id,
            dto.CreatedBy,
            calendar.TeamId
        );
        await _eventRepository.AddAsync(eventAs, cancellationToken);
        return eventAs.Id;
    }

    public async Task<List<EventSummaryDto>> GetEventsForMonthAsync(int calendarId, int month, int year,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetEventsForMonthAsync(calendarId, month, year, cancellationToken);

        return events.Select(e => new EventSummaryDto
            {
                Id = e.Id,
                Name = e.Name,
                Date = e.Date,
                CreatedBy = e.CreatedBy,
            })
            .ToList();
    }

    public async Task<EventDetailsDto?> GetEventDetailsAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _eventRepository.GetByIdAsync(eventId);
        if (eventEntity == null) return null;

        return new EventDetailsDto
        {
            Id = eventEntity.Id,
            Name = eventEntity.Name,
            Description = eventEntity.Description,
            Date = eventEntity.Date,
            CreatedAt = eventEntity.CreatedAt,
            CreatedBy = eventEntity.CreatedBy,
            Tasks = eventEntity.Tasks.Select(t => new TaskSummaryDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description ?? "",
                Status = t.Status
            }).ToList()
        };
    }

    public async Task<List<EventSummaryDto>> GetEventsForTeamAsync(int teamId)
    {
        var calendar = await _calendarRepository.GetByTeamIdAsync(teamId);
        if (calendar == null) return new List<EventSummaryDto>();

        var events = await _eventRepository.GetEventsForCalendarAsync(calendar.Id);
        return events.Select(e => new EventSummaryDto
        {
            Id = e.Id,
            Name = e.Name,
            Date = e.Date,
            Description = e.Description,
            CreatedAt = e.CreatedAt,
            CreatedBy = e.CreatedBy
        }).ToList();
    }

    // --- ИСПРАВЛЕННЫЙ МЕТОД ---
    public async Task<IEnumerable<EventSummaryDto>> GetEventsForCalendarAsync(DateTime start, DateTime end)
    {
        // Вызываем новый метод репозитория
        var events = await _eventRepository.GetEventsByDateRangeAsync(start, end);

        // Маппим сущности в DTO, используя правильные имена свойств
        return events.Select(e => new EventSummaryDto 
        {
            Id = e.Id,
            Name = e.Name,     // Было Title, исправлено на Name
            Date = e.Date,     // Было EventDate, исправлено на Date
            // Остальные поля по необходимости
        });
    }
}