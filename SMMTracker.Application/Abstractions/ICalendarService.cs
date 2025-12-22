using SMMTracker.Application.Dtos;

namespace SMMTracker.Application.Abstractions;

public interface ICalendarService
{
    Task<(int CalendarId, int TeamId)> GetCalendarInfoByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> CreateCalendarAsync(CreateCalendarDto dto, CancellationToken cancellationToken = default);
    Task<int> GetCalendarIdByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}