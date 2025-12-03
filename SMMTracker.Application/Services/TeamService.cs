using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Interfaces;
using SMMTracker.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Application.Services;

public class TeamService
{
    private readonly IApplicationDbContext _context;

    public TeamService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateTeamAsync(CreateTeamDto dto, int creatorId,
        CancellationToken cancellationToken = default)
    {
        var code = GenerateTeamCode();
        while (await _context.Teams.AnyAsync(t => t.Code == code, cancellationToken))
        {
            code = GenerateTeamCode();
        }

        var team = new Team(dto.Name, code);
        _context.Teams.Add(team);
        await _context.SaveChangesAsync(cancellationToken);

        var userTeam = new UserTeam
        {
            TeamId = team.Id,
            UserId = creatorId,
            Role = TeamRole.Admin,
        };
        _context.UserTeams.Add(userTeam);
        await _context.SaveChangesAsync(cancellationToken);
        return team.Id;
    }

    private string GenerateTeamCode()
    {
        const string symbols = "ABCDEFGHIGKLMNOPQRSTUVWXYZ012345678";
        var random = new Random();
        return new string(Enumerable
            .Repeat(symbols, 5)
            .Select(s => s[random.Next(s.Length)])
            .ToArray());
    }

    public async Task<bool> JoinTeamAsync(JoinTeamDto dto, CancellationToken cancellationToken = default)
    {
        var team = await _context.Teams
            .FirstOrDefaultAsync(t => t.Code == dto.Code, cancellationToken);

        if (team == null)
            return false;

        var userTeam = new UserTeam
        {
            UserId = dto.UserId,
            TeamId = team.Id,
            Role = TeamRole.User,
        };

        _context.UserTeams.Add(userTeam);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task RemoveUserFromTeamAsync(int teamId, int userIdToRemove, int adminId,
        CancellationToken cancellationToken = default)
    {
        var team = await _context.Teams
            .Include(t => t.UserTeams)
            .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);
        if (team == null)
            throw new Exception("Team not found");

        var isAdmin = await _context.UserTeams
            .AnyAsync(ut => ut.TeamId == teamId && ut.UserId == adminId && ut.Role == TeamRole.Admin,
                cancellationToken);
        if (!isAdmin)
            throw new UnauthorizedAccessException("Only admins can remove users from team");

        var userTeam = await _context.UserTeams
            .FirstOrDefaultAsync(ut => ut.TeamId == teamId && ut.UserId == userIdToRemove, cancellationToken);
        
        if (userTeam == null)
            throw new Exception("User is not in the team");
        
        _context.UserTeams.Remove(userTeam);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> LeaveTeamAsync(int teamId, int userId, CancellationToken cancellationToken = default)
    {
        var userTeam = await _context.UserTeams
            .FirstOrDefaultAsync(ut => ut.TeamId == teamId && ut.UserId == userId, cancellationToken);
        
        if (userTeam == null)
            return false;
        
        if (userTeam.Role == TeamRole.Admin)
            throw new Exception("Admin cannot leave team");
        
        _context.UserTeams.Remove(userTeam);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}