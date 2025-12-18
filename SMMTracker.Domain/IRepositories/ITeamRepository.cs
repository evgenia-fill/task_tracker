using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(int teamId);
    Task<Team?> GetByCodeAsync(string code);
    Task<bool> ExistsAsync(int teamId);
    Task<bool> ExistsByCodeAsync(string code);
    Task AddAsync(Team team);
    Task UpdateAsync(Team team);
    Task DeleteAsync(int teamId);
    Task<Team?> GetByIdWithMembersAsync(int teamId);
}