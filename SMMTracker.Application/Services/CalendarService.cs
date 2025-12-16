using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using System.Threading;
using System.Threading.Tasks;

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

    // --- МЕТОД, КОТОРЫЙ ИСПОЛЬЗУЕТ Calendar.cshtml.cs ---
    public async Task<int> GetCalendarIdByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        // Вызываем новый метод из репозитория
        var calendar = await _calendarRepository.GetByUserIdAsync(userId, cancellationToken);
        
        return calendar?.Id ?? 0;
    }

    // --- Существующий метод ---
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