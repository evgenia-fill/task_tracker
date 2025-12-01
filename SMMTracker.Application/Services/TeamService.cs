using SMMTracker.Domain.Entities;

namespace SMMTracker.Application.Services;

using SMMTracker.Application.Dtos;
using SMMTracker.Application.Interfaces;

public class TeamService
{
    private readonly IApplicationDbContext _context;

    public TeamService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateTeamAsync(CreateTeamDto dto, CancellationToken cancellationToken = default)
    {
        var team = new Team(dto.Name);
        _context.Teams.Add(team);
        await _context.SaveChangesAsync(cancellationToken);
        return team.Id;
    }
}