using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IApplicationDbContext _context;

    public EventRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    // ... (ваши существующие методы GetByIdAsync, GetEventsForCalendarAsync и т.д. остаются без изменений) ...

    public async Task<Event?> GetByIdAsync(int eventId)
    {
        return await _context.Events
            .Include(e => e.Tasks)
            .Include(e => e.Calendar)
            .Include(e => e.Team)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken: cancellationToken);
    }

    public async Task<List<Event>> GetEventsForCalendarAsync(int calendarId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.CalendarId == calendarId)
            .Include(e => e.Tasks)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<Event>> GetEventsForMonthAsync(int calendarId, int month, int year,
        CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.CalendarId == calendarId && e.Date.Year == year && e.Date.Month == month)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<List<Event>> GetEventsForTeamAsync(int teamId)
    {
        return await _context.Events
            .Where(e => e.Calendar.TeamId == teamId)
            .Include(e => e.Tasks)
            .Include(e => e.Calendar)
            .AsSplitQuery()
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Event eventEntity)
    {
        await _context.Events.AddAsync(eventEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Event eventEntity, CancellationToken cancellationToken = default)
    {
        _context.Events.Update(eventEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int eventId, CancellationToken cancellationToken = default)
    {
        var eventEntity = await _context.Events.FindAsync(eventId);
        if (eventEntity != null)
        {
            _context.Events.Remove(eventEntity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // --- ВОТ НОВЫЙ МЕТОД ---
    public async Task<List<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Events
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .ToListAsync();
    }

    // --- ВОТ НОВЫЙ МЕТОД ---
    public async Task<List<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Events
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .ToListAsync();
    }
}