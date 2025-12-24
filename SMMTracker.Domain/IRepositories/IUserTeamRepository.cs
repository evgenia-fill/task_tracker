using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface IUserTeamRepository 
{
    Task<UserTeam?> GetUserTeamAsync(int teamId, int userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int userTeamId, CancellationToken cancellationToken = default);
    Task AddAsync(UserTeam userTeam, CancellationToken cancellationToken = default);
    Task DeleteAsync(int userTeamId, CancellationToken cancellationToken = default);
    Task<bool> IsUserAdminAsync(int teamId, int userid, CancellationToken cancellationToken = default);
    Task<List<UserTeam>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
}