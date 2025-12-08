using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Interfaces;

namespace SMMTracker.Application.Services;

public class EventService
{
    private readonly IApplicationDbContext _context;

    public EventService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateEventAsync(CreateEventDto dto,
        CancellationToken cancellationToken = default)
    {
        var eventAs = new Event(
            dto.Name,
            dto.Description,
            dto.Date,
            dto.CalendarId
        );
        _context.Events.Add(eventAs);
        await _context.SaveChangesAsync(cancellationToken);
        return eventAs.Id;
    }

    public async Task<List<EventSummaryDto>> GetEventsForMonthAsync(int calendarId, int month, int year,
        CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.CalendarId == calendarId && e.Date.Year == year && e.Date.Month == month)
            .Select(e => new EventSummaryDto
            {
                Id = e.Id,
                Name = e.Name,
                Date = e.Date
            })
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<EventDetailsDto?> GetEventDetailsAsync(int eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Events
            .Where(e => e.Id == eventId)
            .Include(e => e.Tasks)
            .Select(e =>
                new EventDetailsDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Description = e.Description,
                    Date = e.Date,
                    Tasks = e.Tasks
                        .Select(t => new TaskSummaryDto
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Status = (TaskStatus)t.Status
                        }).ToList()
                })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}