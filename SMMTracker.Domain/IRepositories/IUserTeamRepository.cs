using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface IUserTeamRepository 
{
    Task<UserTeam?> GetUserTeamAsync(int teamId, int userId);
    Task<bool> ExistsAsync(int userTeamId);
    Task AddAsync(UserTeam userTeam);
    Task DeleteAsync(int userTeamId);
    Task<bool> IsUserAdminAsync(int teamId, int userid);
}