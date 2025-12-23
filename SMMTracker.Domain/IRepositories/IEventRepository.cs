using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(int eventId);
    Task<List<Event>> GetEventsForCalendarAsync(int calendarId);
    Task<List<Event>> GetEventsForMonthAsync(int calendarId, int month, int year);
    Task AddAsync(Event eventEntity);
    Task UpdateAsync(Event eventEntity);
    Task DeleteAsync(int eventId);
}