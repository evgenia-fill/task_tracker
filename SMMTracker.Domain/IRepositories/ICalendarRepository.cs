using SMMTracker.Domain.Entities;
using Task =System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface ICalendarRepository
{
    Task<Calendar?> GetByIdAsync(int calendarId);
    Task<Calendar?> GetByTeamIdAsync(int teamId);
    Task<List<Calendar>> GetAllAsync();
    Task AddAsync(Calendar calendar);
    Task<Calendar?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(Calendar calendar);
    Task DeleteAsync(int calendarId);
    Task<bool> ExistsAsync(int calendarId);
}