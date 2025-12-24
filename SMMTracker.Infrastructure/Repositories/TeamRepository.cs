using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class TeamRepository : ITeamRepository
{
    private IApplicationDbContext _context;

    public TeamRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.UserTeams)
            .ThenInclude(ut => ut.User) 
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<Team?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(team => team.Code == code, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(int teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Teams.AnyAsync(team => team.Id == teamId, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.Teams.AnyAsync(team => team.Code == code, cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await _context.Teams.AddAsync(team, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int teamId, CancellationToken cancellationToken = default)
    {
        var team = await _context.Teams.FindAsync(teamId);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
    
    public async Task<Team?> GetByIdWithMembersAsync(int teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.UserTeams)
            .ThenInclude(ut => ut.User)
            .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken: cancellationToken);
    }
}