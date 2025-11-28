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
        
        builder.SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        
        IConfiguration configuration = builder.Build();
        
        if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")))
        {
            Console.WriteLine($"Критическая ошибка: Файл конфигурации не найден по пути: {Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")}");
            return;
        }

        var token = configuration.GetValue<string>("TelegramBotToken");

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Критическая ошибка: Токен бота (TelegramBotToken) не найден в файле appsettings.json!");
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

        Console.WriteLine("Бот запущен. Ожидание завершения...");
        
        await Task.Delay(Timeout.Infinite);
    }
}