using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using SMMTracker.Infrastructure.Data.DataContext;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class CalendarRepository : ICalendarRepository
{
    private readonly ApplicationDbContext _context;

    public CalendarRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Calendar?> GetByIdAsync(int calendarId)
    {
        return await _context.Calendars
            .Include(c => c.Team)
            .Include(c => c.Events)
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == calendarId);
    }
    
    // Внутри класса CalendarRepository (который использует DbContext)

    public async Task<Calendar?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Используем DbContext (_context) для выполнения запроса:
        return await _context.Calendars 
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<Calendar?> GetByTeamIdAsync(int teamId)
    {
        return await _context.Calendars
            .Include(c => c.Team)
            .Include(c => c.Events)
            .Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.TeamId == teamId);
    }

    public async Task<List<Calendar>> GetAllAsync()
    {
        return await _context.Calendars
            .Include(c => c.Team)
            .ToListAsync();
    }

    public async Task AddAsync(Calendar calendar)
    {
        await _context.Calendars.AddAsync(calendar);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Calendar calendar)
    {
        _context.Calendars.Update(calendar);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int calendarId)
    {
        var calendar = await _context.Calendars.FindAsync(calendarId);
        if (calendar != null)
        {
            _context.Calendars.Remove(calendar);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int calendarId)
    {
        return await _context.Calendars.AnyAsync(c => c.Id == calendarId);
    }
}