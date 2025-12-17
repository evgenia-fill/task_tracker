using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Services;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Repositories;
using SMMTracker.Domain.IRepositories;

namespace SMMTracker.TgBot;

class Program
{
    const string token = "8450218559:AAGCQdk6hnrtP8aFZpZM-bCc7tCWeKNWaIE";

    public static async Task Main()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            var solutionDir = Directory.GetParent(AppContext.BaseDirectory)!
                .Parent!.Parent!.Parent!.Parent!.FullName;
            
            var dbPath = Path.Combine(solutionDir, "SharedDatabase", "DataBase.db");
            var dir = Path.GetDirectoryName(dbPath);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir!);
            }

            connectionString = $"Data Source={dbPath}";
        }

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connectionString)
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.MigrateAsync();

        IUserRepository userRepository = new UserRepository(context);
        IUserService userService = new UserService(userRepository);

        var bot = new TelegramBotService(token, userService);
        await bot.StartAsync(CancellationToken.None);

        await Task.Delay(-1);
    }
}