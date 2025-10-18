// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Components.Web;
// using BlazorApp1.Data;
//
// var builder = WebApplication.CreateBuilder(args);
//
// // Add services to the container.
// builder.Services.AddRazorPages();
// builder.Services.AddServerSideBlazor();
// builder.Services.AddSingleton<WeatherForecastService>();
//
// var app = builder.Build();
//
// // Configure the HTTP request pipeline.
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Error");
//     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//     app.UseHsts();
// }
//
// app.UseHttpsRedirection();
//
// app.UseStaticFiles();
//
// app.UseRouting();
//
// app.MapBlazorHub();
// app.MapFallbackToPage("/_Host");
//
// app.Run();

using BlazorApp1.Bot;
using static BlazorApp1.Bot.UserManager;
using Microsoft.Data.Sqlite;

class Program
{
    const string token = "8450218559:AAGCQdk6hnrtP8aFZpZM-bCc7tCWeKNWaIE";
    public static async Task Main()
    {
        var db = new DatabaseService();
        var dbPath = Path.Combine(AppContext.BaseDirectory, "DataBase.db");
        var userManager = new UserManager(dbPath);
        
        var bot = new TelegramBotService(token, userManager);

        await bot.StartAsync(CancellationToken.None);

        Console.WriteLine("Бот запущен. Нажмите любую клавишу для выхода...");
        Console.ReadKey();

        Console.WriteLine(ShowBd());
    }

    private static string ShowBd()
    {
        var list = new List<string>();
        var dbPath1 = @"C:\Users\_\Desktop\task_tracker_proj\BlazorApp1\bin\Debug\net6.0\DataBase.db";
        using var connection = new SqliteConnection($"Data Source={dbPath1}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Users";
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            list.Add($"{reader["TelegramId"]}, {reader["FullName"]}, {reader["UniqueCode"]}");
        }

        return string.Join("\n", list);
    }
}