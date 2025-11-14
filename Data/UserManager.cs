using Microsoft.Data.Sqlite;
using Data.models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Data.DataContext;

namespace Data;

public class UserManager
{
    private readonly ApplicationDbContext context;

    public UserManager(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<User> FindOrCreateUserAsync(User tgUser)
    {
        ArgumentNullException.ThrowIfNull(tgUser);
        var user = await FindUserByTgIdAsync(tgUser.TelegramId);
        if (user != null) return user;
        await context.Users.AddAsync(tgUser);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> FindUserByTgIdAsync(long telegramId)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
    }
}