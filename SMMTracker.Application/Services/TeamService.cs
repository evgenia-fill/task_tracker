using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Enums;
using SMMTracker.Domain.IRepositories;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Application.Services;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUserTeamRepository _userTeamRepository;
    private readonly ICalendarRepository _calendarRepository;

    public TeamService(ITeamRepository teamRepository, IUserTeamRepository userTeamRepository,
        ICalendarRepository calendarRepository)
    {
        _teamRepository = teamRepository;
        _userTeamRepository = userTeamRepository;
        _calendarRepository = calendarRepository;
    }

    public async Task<int> CreateTeamAsync(CreateTeamDto dto, int creatorId,
        CancellationToken cancellationToken = default)
    {
        var code = GenerateTeamCode();
        while (await _teamRepository.ExistsByCodeAsync(code))
        {
            code = GenerateTeamCode();
        }

        var team = new Team(dto.Name, code, dto.Description);
        await _teamRepository.AddAsync(team);

        var calendar = new Calendar(team.Id);
        await _calendarRepository.AddAsync(calendar);

        var userTeam = new UserTeam
        {
            TeamId = team.Id,
            UserId = creatorId,
            Role = TeamRole.Admin,
            Team = team 
        };

        team.UserTeams.Add(userTeam);

        await _userTeamRepository.AddAsync(userTeam);

        return team.Id;
    }


    private static string GenerateTeamCode()
    {
        const string symbols = "ABCDEFGHIGKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable
            .Repeat(symbols, 5)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }

    public async Task<bool> JoinTeamAsync(JoinTeamDto dto, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByCodeAsync(dto.Code);

        if (team == null)
            return false;

        var userTeam = new UserTeam
        {
            UserId = dto.UserId,
            TeamId = team.Id,
            Role = TeamRole.User,
        };

        await _userTeamRepository.AddAsync(userTeam);

        return true;
    }

    public async Task RemoveUserFromTeamAsync(int teamId, int userIdToRemove, int adminId,
        CancellationToken cancellationToken = default)
    {
        if (!await _teamRepository.ExistsAsync(teamId))
            throw new Exception("Team not found");

        if (!await _userTeamRepository.IsUserAdminAsync(teamId, adminId))
            throw new UnauthorizedAccessException("Only admins can remove users from team");

        var userTeam = await _userTeamRepository.GetUserTeamAsync(teamId, userIdToRemove);

        if (userTeam == null)
            throw new Exception("User is not in the team");

        await _userTeamRepository.DeleteAsync(userTeam.Id);
    }

    public async Task<bool> LeaveTeamAsync(int teamId, int userId, CancellationToken cancellationToken = default)
    {
        var userTeam = await _userTeamRepository.GetUserTeamAsync(teamId, userId);

        if (userTeam == null)
            return false;

        if (userTeam.Role == TeamRole.Admin)
            throw new Exception("Admin cannot leave team");

        await _userTeamRepository.DeleteAsync(userTeam.Id);
        return true;
    }

    public async Task<List<TeamDto>> GetTeamsForUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var userTeams = await _userTeamRepository.GetByUserIdAsync(userId);

        return userTeams.Select(ut => new TeamDto
        {
            Id = ut.Team.Id,
            Name = ut.Team.Name,
            IsOwner = ut.Role == TeamRole.Admin,
            InvitationCode = ut.Team.Code,
        }).ToList();
    }

    public async Task<List<TeamMemberDto>> GetTeamMembersAsync(int teamId)
    {
        var team = await _teamRepository.GetByIdWithMembersAsync(teamId);
        if (team == null) return new List<TeamMemberDto>();

        return team.UserTeams.Select(ut => new TeamMemberDto
        {
            UserId = ut.UserId,
            FirstName = ut.User?.FirstName ?? "",
            LastName = ut.User?.LastName ?? "",
            Username = ut.User?.UserName ?? "",
            Role = ut.Role switch
            {
                TeamRole.Admin => "Админ",
                TeamRole.User => "Участник",
                _ => "Участник"
            }
        }).ToList();
    }

    public async Task<bool> IsUserAdminAsync(int teamId, int userId)
    {
        return await _userTeamRepository.IsUserAdminAsync(teamId, userId);
    }

    public async Task<TeamDetailsDto> GetTeamDetailsAsync(int teamId)
    {
        var team = await _teamRepository.GetByIdWithMembersAsync(teamId);

        if (team == null) 
            return null;

        var teamDetailsDto = new TeamDetailsDto
        {
            Id = team.Id,
            Name = team.Name,
            InvitationCode = team.Code,
            Members = team.UserTeams.Select(ut => new TeamMemberDto
            {
                UserId = ut.UserId,
                FirstName = ut.User?.FirstName ?? "",
                LastName = ut.User?.LastName ?? "",
                Username = ut.User?.UserName ?? "",
                Role = ut.Role switch
                {
                    TeamRole.Admin => "Админ",
                    TeamRole.User => "Участник",
                    _ => "Участник"
                }   
            }).ToList()
        };

        return teamDetailsDto;
    }
    public async Task UpdateTeamAsync(int teamId, string newName, string newDescription, int adminId,
        CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
            throw new Exception("Team not found");
        
        if (!await _userTeamRepository.IsUserAdminAsync(teamId, adminId))
            throw new UnauthorizedAccessException("Only admins can update the team");
        
        team.Name = newName;
        team.Description = newDescription;
        await _teamRepository.UpdateAsync(team);
    }


    public async Task<CalendarDto?> GetCalendarForTeamAsync(int teamId)
    {
        var calendar = await _calendarRepository.GetByTeamIdAsync(teamId);
    
        return calendar == null ? null : new CalendarDto { Id = calendar.Id };
    }
}