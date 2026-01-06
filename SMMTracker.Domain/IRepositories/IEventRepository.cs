using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int eventId, CancellationToken cancellationToken = default);
    Task<List<Event>> GetEventsForCalendarAsync(int calendarId, CancellationToken cancellationToken = default);

    Task<List<Event>> GetEventsForMonthAsync(int calendarId, int month, int year,
        CancellationToken cancellationToken = default);

    Task AddAsync(Event eventEntity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Event eventEntity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int eventId, CancellationToken cancellationToken = default);
}