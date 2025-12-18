using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<User?> GetByTelegramIdAsync(long telegramId);
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int userId);
    Task<bool> ExistsAsync(int userId);
}