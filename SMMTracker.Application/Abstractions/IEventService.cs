using SMMTracker.Application.Dtos;

namespace SMMTracker.Application.Abstractions;

public interface IEventService
{
    Task<int> CreateEventAsync(CreateEventDto dto, CancellationToken cancellationToken = default);

    Task<List<EventSummaryDto>> GetEventsForMonthAsync(int calendarId, int month, int year,
        CancellationToken cancellationToken = default);

    Task<EventDetailsDto?> GetEventDetailsAsync(int eventId, CancellationToken cancellationToken = default);
    Task<List<EventSummaryDto>> GetEventsForTeamAsync(int teamId);
}