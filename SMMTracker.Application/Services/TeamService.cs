using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Interfaces;
using SMMTracker.Domain.Enums;

namespace SMMTracker.Application.Services;

public class TeamService
{
    private readonly IApplicationDbContext _context;

    public TeamService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateTeamAsync(CreateTeamDto dto, CancellationToken cancellationToken = default)
    {
        var code = GenerateTeamCode();
        while (await _context.Teams.AnyAsync(t => t.Code == code, cancellationToken))
        {
            code = GenerateTeamCode();
        }
        var team = new Team(dto.Name, code);
        _context.Teams.Add(team);
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
    
}