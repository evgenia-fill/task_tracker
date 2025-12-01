using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Interfaces;

namespace SMMTracker.Application.Services;

public class CalendarService
{
    private readonly IApplicationDbContext _context;

    public CalendarService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateCalendarAsync(CreateCalendarDto dto,
        CancellationToken cancellationToken = default)
    {
        var teamExists = await _context.Teams.AnyAsync(t => t.Id == dto.TeamId, cancellationToken);
        if (!teamExists)
        {
            throw new Exception($"Команда с Id={dto.TeamId} не найдена.");
        }

        var calendar = new Calendar(dto.TeamId);
        _context.Calendars.Add(calendar);
        await _context.SaveChangesAsync(cancellationToken);
        return calendar.Id;
    }
}