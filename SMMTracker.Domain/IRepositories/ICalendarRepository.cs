using SMMTracker.Domain.Entities;
using Task =System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface ICalendarRepository
{
    Task<Calendar?> GetByIdAsync(int calendarId, CancellationToken cancellationToken = default);
    Task<Calendar?> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default);
    Task<List<Calendar>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Calendar calendar, CancellationToken cancellationToken = default);
    Task<Calendar?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Calendar calendar, CancellationToken cancellationToken = default);
    Task DeleteAsync(int calendarId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int calendarId, CancellationToken cancellationToken = default);
}