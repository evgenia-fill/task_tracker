using Microsoft.EntityFrameworkCore; 
using SMMTracker.Application.Interfaces.Repositories;
using SMMTracker.Domain.Entities;
using SMMTracker.Infrastructure.Data.DataContext;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByTelegramIdAsync(long telegramId)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.TelegramId == telegramId);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}