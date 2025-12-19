using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Services;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Repositories;
using SMMTracker.Domain.IRepositories;

namespace SMMTracker.TgBot;

static class Program
{
    public static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Secrets.json", optional: false)
            .Build();

        var token = configuration["TelegramBotToken"];
        
        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("Токен не найден в файле appsettings.Secrets.json");
            return;
        }
        Console.WriteLine($"Токен найден");
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("путь к бд не найден в конфиге");
            return;
        }
        
        Console.WriteLine($"путь к бд {connectionString}");
        
        var services = new ServiceCollection();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();
        
        var serviceProvider = services.BuildServiceProvider();

        try
        {
            var userService = serviceProvider.GetRequiredService<IUserService>();
            var bot = new TelegramBotService(token, userService);
            
            await bot.StartAsync(CancellationToken.None);

            Console.WriteLine("Бот запущен");
            Console.WriteLine("/start в @SmmTrackerTestBot_bot");
            
            await Task.Delay(-1, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}