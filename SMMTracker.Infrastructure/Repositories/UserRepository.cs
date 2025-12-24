using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IApplicationDbContext _context;

    public UserRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    }

    public async Task<User?> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.TelegramId == telegramId,
            cancellationToken: cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower(), cancellationToken: cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(default);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users.AnyAsync(u => u.Id == userId, cancellationToken: cancellationToken);
    }
}