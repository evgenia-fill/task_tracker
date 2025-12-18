using Microsoft.AspNetCore.Builder;
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

    public async Task<Team?> GetByIdAsync(int teamId)
    {
        return await _context.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(team => team.Id == teamId);
    }

    public async Task<Team?> GetByCodeAsync(string code)
    {
        return await _context.Teams
            .AsNoTracking()
            .FirstOrDefaultAsync(team => team.Code == code);
    }

    public async Task<bool> ExistsAsync(int teamId)
    {
        return await _context.Teams.AnyAsync(team => team.Id == teamId);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _context.Teams.AnyAsync(team => team.Code == code);
    }

    public async Task AddAsync(Team team)
    {
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync(default);
    }

    public async Task UpdateAsync(Team team)
    {
        _context.Teams.Update(team);
        await _context.SaveChangesAsync(default);
    }

    public async Task DeleteAsync(int teamId)
    {
        var team = await _context.Teams.FindAsync(teamId);
        if (team != null)
        {
            _context.Teams.Remove(team);
            await _context.SaveChangesAsync(default);
        }
    }

    public async Task<Team?> GetByIdWithMembersAsync(int teamId)
    {
        return await _context.Teams
            .Include(t => t.UserTeams)
            .ThenInclude(ut => ut.User)
            .FirstOrDefaultAsync(t => t.Id == teamId);
    }
}