using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private readonly IApplicationDbContext _context;

    public CalendarRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Calendar?> GetByIdAsync(int calendarId, CancellationToken cancellationToken = default)
    {
        return await _context.Calendars
            .Include(c => c.Team)
            .Include(c => c.Events)
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == calendarId, cancellationToken: cancellationToken);
    }

    public async Task<Calendar?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Calendars 
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<Calendar?> GetByTeamIdAsync(int teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Calendars
            .Include(c => c.Team)
            .Include(c => c.Events)
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.TeamId == teamId, cancellationToken: cancellationToken);
    }

    public async Task<List<Calendar>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Calendars
            .Include(c => c.Team)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Calendar calendar, CancellationToken cancellationToken = default)
    {
        await _context.Calendars.AddAsync(calendar);
        // await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Calendar calendar, CancellationToken cancellationToken = default)
    {
        _context.Calendars.Update(calendar);
        // await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int calendarId, CancellationToken cancellationToken = default)
    {
        var calendar = await _context.Calendars.FindAsync(calendarId);
        if (calendar != null)
        {
            _context.Calendars.Remove(calendar);
            // await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int calendarId, CancellationToken cancellationToken = default)
    {
        return await _context.Calendars.AnyAsync(c => c.Id == calendarId, cancellationToken: cancellationToken);
    }
}