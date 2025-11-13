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

    public async Task<User> FindOrCreateUserAsync(TelegramUser tgUser)
    {
        ArgumentNullException.ThrowIfNull(tgUser);
        var user = await FindUserByTgIdAsync(tgUser.id);
        if (user != null) return user;

        user = new User
        {
            TelegramId = tgUser.id,
            FirstName = tgUser.first_name,
            LastName = tgUser.last_name,
            UserName = !string.IsNullOrEmpty(tgUser.username) ? tgUser.username : tgUser.first_name
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<User?> FindUserByTgIdAsync(long telegramId)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
    }

}