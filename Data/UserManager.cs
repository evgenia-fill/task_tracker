using Microsoft.Data.Sqlite;
using Data.models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Data.DataContext;
using Task = System.Threading.Tasks.Task;

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
        return tgUser;
    }

    private async Task<User?> FindUserByTgIdAsync(long telegramId)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
    }
    
    public async Task DeleteUserAsync(long telegramId)
    {
        var user = await FindUserByTgIdAsync(telegramId);
        if (user != null)
        {
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}