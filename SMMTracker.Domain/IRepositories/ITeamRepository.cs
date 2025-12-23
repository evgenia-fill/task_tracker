using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface ITeamRepository
{
    Task<Team?> GetByIdAsync(int teamId, CancellationToken cancellationToken = default);
    Task<Team?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int teamId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Team team, CancellationToken cancellationToken = default);
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);
    Task DeleteAsync(int teamId, CancellationToken cancellationToken = default);
    Task<Team?> GetByIdWithMembersAsync(int teamId, CancellationToken cancellationToken = default);
}