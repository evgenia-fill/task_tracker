using SMMTracker.Application.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace SMMTracker.Application.Abstractions;

public interface ICalendarService
{
    // Метод, который нужен Calendar.cshtml.cs для получения ID календаря
    Task<int> GetCalendarIdByUserIdAsync(string userId, CancellationToken cancellationToken = default); 

    // Метод, который уже реализован в вашем CalendarService.cs
    Task<int> CreateCalendarAsync(CreateCalendarDto dto, CancellationToken cancellationToken = default);
}