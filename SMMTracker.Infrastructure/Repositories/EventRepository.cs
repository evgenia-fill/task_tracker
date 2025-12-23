using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using SMMTracker.Infrastructure.Data.DataContext;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly ApplicationDbContext _context;

    public EventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Event?> GetByIdAsync(int eventId)
    {
        return await _context.Events
            .Include(e => e.Tasks)
            .Include(e => e.Calendar)
            .Include(e => e.Team)
            .FirstOrDefaultAsync(e => e.Id == eventId);
    }

    public async Task<List<Event>> GetEventsForCalendarAsync(int calendarId)
    {
        return await _context.Events
            .Where(e => e.CalendarId == calendarId)
            .Include(e => e.Tasks)
            .ToListAsync();
    }

    public async Task<List<Event>> GetEventsForMonthAsync(int calendarId, int month, int year)
    {
        return await _context.Events
            .Where(e => e.CalendarId == calendarId && e.Date.Year == year && e.Date.Month == month)
            .ToListAsync();
    }
    public async Task<List<Event>> GetEventsForTeamAsync(int teamId)
    {
        return await _context.Events
            .Where(e => e.Calendar.TeamId == teamId) 
            .Include(e => e.Tasks)                   
            .Include(e => e.Calendar)                 
            .AsSplitQuery()                           
            .ToListAsync();
    }
    public async Task AddAsync(Event eventEntity)
    {
        await _context.Events.AddAsync(eventEntity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Event eventEntity)
    {
        _context.Events.Update(eventEntity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int eventId)
    {
        var eventEntity = await _context.Events.FindAsync(eventId);
        if (eventEntity != null)
        {
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync();
        }
    }
}