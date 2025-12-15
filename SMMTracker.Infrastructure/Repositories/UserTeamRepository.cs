using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.Enums;
using SMMTracker.Domain.IRepositories;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class UserTeamRepository : IUserTeamRepository
{
    private readonly IApplicationDbContext _context;

    public UserTeamRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserTeam?> GetUserTeamAsync(int teamId, int userId)
    {
        return await _context.UserTeams.FirstOrDefaultAsync(ut => ut.TeamId == teamId && ut.UserId == userId);
    }

    public async Task<bool> ExistsAsync(int userTeamId)
    {
        return await _context.UserTeams.AnyAsync(userTeam => userTeam.Id == userTeamId);
    }

    public async Task AddAsync(UserTeam userTeam)
    {
        await _context.UserTeams.AddAsync(userTeam);
        await _context.SaveChangesAsync(default);
    }

    public async Task DeleteAsync(int userTeamId)
    {
        var userTeam = await _context.UserTeams.FindAsync(userTeamId);
        if (userTeam != null)
        {
            _context.UserTeams.Remove(userTeam);
            await _context.SaveChangesAsync(default);
        }
    }

    public async Task<bool> IsUserAdminAsync(int teamId, int userId)
    {
        return await _context.UserTeams
            .AnyAsync(ut => ut.TeamId == teamId && ut.UserId == userId && ut.Role == TeamRole.Admin);
    }
}