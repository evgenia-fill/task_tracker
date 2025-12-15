using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.IRepositories;

namespace SMMTracker.Application.Services;

public class CalendarService : ICalendarService
{
    private readonly ICalendarRepository _calendarRepository;
    private readonly ITeamRepository _teamRepository;

    public CalendarService(ICalendarRepository calendarRepository, ITeamRepository teamRepository)
    {
        _calendarRepository = calendarRepository;
        _teamRepository = teamRepository;
    }

    public async Task<int> CreateCalendarAsync(CreateCalendarDto dto,
        CancellationToken cancellationToken = default)
    {
        var teamExists = await _teamRepository.ExistsAsync(dto.TeamId);
        if (!teamExists)
            throw new Exception($"Команда с Id={dto.TeamId} не найдена.");

        var calendar = new Calendar(dto.TeamId);
        await _calendarRepository.AddAsync(calendar);
        return calendar.Id;
    }
}