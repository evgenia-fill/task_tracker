using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Interfaces; 
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Services;
using SMMTracker.TgBot;

namespace SMMTracker.TgBot;
class Program
{
    const string token = "8450218559:AAGCQdk6hnrtP8aFZpZM-bCc7tCWeKNWaIE";

    public static async Task Main()
    {
        var solutionDir = Directory.GetParent(AppContext.BaseDirectory)
            .Parent.Parent.Parent.Parent.FullName;
        
        var dbPath = Path.Combine(solutionDir, "SharedDatabase", "DataBase.db");

        Console.WriteLine("DB Path: " + dbPath);
        var dir = Path.GetDirectoryName(dbPath);
        Console.WriteLine("Folder exists: " + Directory.Exists(dir));

        if (!Directory.Exists(dir))
        {
            Console.WriteLine("Creating directory manually...");
            Directory.CreateDirectory(dir);
        }
        var connectionString = $"Data Source={dbPath}";

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connectionString)
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.MigrateAsync();

        IUserManager userManager = new UserManager(context); 

        var bot = new TelegramBotService(token, userManager); 
        await bot.StartAsync(CancellationToken.None);

        Console.WriteLine("Бот запущен. Нажмите любую клавишу для выхода...");
        Console.ReadKey();

        Console.WriteLine("\nСодержимое базы данных:");
        Console.WriteLine(await ShowDbAsync(context));
    }

    private static async Task<string> ShowDbAsync(ApplicationDbContext context)
    {
        var users = await context.Users.ToListAsync();
        if (!users.Any())
            return "База данных пуста";

        var list = users.Select(u =>
            $"ID: {u.Id}, TelegramId: {u.TelegramId}, Имя: {u.FirstName}, Фамилия: {u.LastName}, Username: {u.UserName}" 
        );

        return string.Join("\n", list);
    }
}