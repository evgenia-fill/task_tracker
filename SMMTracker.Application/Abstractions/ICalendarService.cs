using SMMTracker.Application.Dtos;

namespace SMMTracker.Application.Abstractions;

public interface ICalendarService
{
    Task<int> CreateCalendarAsync(CreateCalendarDto dto, CancellationToken cancellationToken = default);
}