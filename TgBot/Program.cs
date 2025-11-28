using Data;
using Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Tg_Bot;

class Program
{
    public static async Task Main()
    {
        /*var solutionDir = Directory.GetParent(AppContext.BaseDirectory)
                              ?.Parent?.Parent?.Parent?.Parent?.FullName
                          ?? throw new InvalidOperationException("Не удалось найти папку решения");*/

        var dataProjectDir = Path.Combine(AppContext.BaseDirectory, "Data");
        var dbPath = Path.Combine(dataProjectDir, "DataBase.db");

        var connectionString = $"Data Source={dbPath}";

        Console.WriteLine($"Бот будет использовать базу данных по пути: {dbPath}");

        var builder = new ConfigurationBuilder();
        var configPath =
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../BlazorApp1/appsettings.json"));

        if (!File.Exists(configPath))
        {
            Console.WriteLine($"Критическая ошибка: Файл конфигурации не найден по пути: {configPath}");
            Console.ReadKey();
            return;
        }

        builder.AddJsonFile(configPath, optional: false);
        IConfiguration configuration = builder.Build();
        var token = configuration.GetValue<string>("TelegramBotToken");

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Токен бота (TelegramBotToken) не найден в файле appsettings.json!");
            Console.ReadKey();
            return;
        }


        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connectionString)
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.MigrateAsync();

        var userManager = new UserManager(context);

        var bot = new TelegramBotService(token, userManager);
        await bot.StartAsync(CancellationToken.None);

        Console.WriteLine("Бот запущен. Нажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}