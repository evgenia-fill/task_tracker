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
}