using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByTelegramIdAsync(long telegramId);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}