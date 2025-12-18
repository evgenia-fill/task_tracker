using SMMTracker.Application.Dtos;
using Task = System.Threading.Tasks.Task;
using System.Threading;

namespace SMMTracker.Application.Abstractions;

public interface ITeamService
{
    Task<int> CreateTeamAsync(CreateTeamDto dto, int creatorId, CancellationToken cancellationToken = default);

    Task<bool> JoinTeamAsync(JoinTeamDto dto, CancellationToken cancellationToken = default);

    Task RemoveUserFromTeamAsync(int teamId, int userIdToRemove, int adminId,
        CancellationToken cancellationToken = default);

    Task<bool> LeaveTeamAsync(int teamId, int userId, CancellationToken cancellationToken = default);
    Task<List<TeamDto>> GetTeamsForUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> IsUserAdminAsync(int teamId, int userId);
    Task<TeamDetailsDto> GetTeamDetailsAsync(int teamId);
    Task<CalendarDto?> GetCalendarForTeamAsync(int teamId);
}