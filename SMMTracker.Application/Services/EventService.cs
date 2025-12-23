using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.IRepositories;

namespace SMMTracker.Application.Services;

public class EventService : IEventService
{
    private readonly IEventRepository _eventRepository;
    private readonly ICalendarRepository _calendarRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EventService(IEventRepository eventRepository, ICalendarRepository calendarRepository,
        IUnitOfWork unitOfWork)
    {
        _eventRepository = eventRepository;
        _calendarRepository = calendarRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> CreateEventAsync(CreateEventDto dto,
        CancellationToken cancellationToken = default)
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
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return eventAs.Id;
    }

    public async Task<List<EventSummaryDto>> GetEventsForMonthAsync(int calendarId, int month, int year,
        CancellationToken cancellationToken = default)
    {
        var events = await _eventRepository.GetEventsForMonthAsync(calendarId, month, year, cancellationToken);

        return events
            .Select(e => new EventSummaryDto
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
        var eventEntity = await _eventRepository.GetByIdAsync(eventId, cancellationToken);

        if (eventEntity == null)
            return null;

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
                Status = t.Status
            }).ToList()
        };

    }

    public async Task<List<EventSummaryDto>> GetEventsForTeamAsync(int teamId)
    {
        var calendar = await _calendarRepository.GetByTeamIdAsync(teamId);
        if (calendar == null)
        {
            return [];
        }

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
}