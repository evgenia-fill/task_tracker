// using Data;
// using Microsoft.Data.Sqlite;
// using Tg_Bot;
//
// class Program
// {
//     const string token = "8450218559:AAGCQdk6hnrtP8aFZpZM-bCc7tCWeKNWaIE";
//     public static async Task Main()
//     {
//         var db = new DatabaseService();
//         var dbPath = Path.Combine(AppContext.BaseDirectory, "DataBase.db");
//         var userManager = new UserManager(dbPath);
//         
//         var bot = new TelegramBotService(token, userManager);
//
//         await bot.StartAsync(CancellationToken.None);
//
//         Console.WriteLine("Бот запущен. Нажмите любую клавишу для выхода...");
//         Console.ReadKey();
//
//         Console.WriteLine(ShowBd());
//     }
//
//     private static string ShowBd()
//     {
//         var list = new List<string>();
//         var dbPath1 = @"C:\Users\_\Desktop\task_tracker_proj\BlazorApp1\bin\Debug\net6.0\DataBase.db";
//         using var connection = new SqliteConnection($"Data Source={dbPath1}");
//         connection.Open();
//
//         var command = connection.CreateCommand();
//         command.CommandText = "SELECT * FROM Users";
//         using var reader = command.ExecuteReader();
//
//         while (reader.Read())
//         {
//             list.Add($"{reader["TelegramId"]}, {reader["FullName"]}, {reader["UniqueCode"]}");
//         }
//
//         return string.Join("\n", list);
//     }
// }

using Data;
using Microsoft.Data.Sqlite;
using Tg_Bot;

class Program
{
    const string token = "8450218559:AAGCQdk6hnrtP8aFZpZM-bCc7tCWeKNWaIE";

    public static async Task Main()
    {
        // var dbPath = Path.Combine(AppContext.BaseDirectory, "DataBase.db");
        var dbPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
            @"../../../../SharedDatabase/DataBase.db"));
        var connectionString = $"Data Source={dbPath}";

        var dbService = new DatabaseService(connectionString);
        dbService.Initialize();

        var userManager = new UserManager(connectionString);

        var bot = new TelegramBotService(token, userManager);
        await bot.StartAsync(CancellationToken.None);

        Console.WriteLine("Бот запущен. Нажмите любую клавишу для выхода...");
        Console.ReadKey();

        Console.WriteLine("\nСодержимое базы данных:");
        Console.WriteLine(ShowBd(connectionString));
    }

    private static string ShowBd(string connectionString)
    {
        var list = new List<string>();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM Users";
        using var reader = command.ExecuteReader();

        if (!reader.HasRows)
        {
            return "База данных пуста";
        }

        while (reader.Read())
        {
            list.Add($"ID: {reader["TelegramId"]}, Имя: {reader["FullName"]}, Код: {reader["UniqueCode"]}");
        }

        return string.Join("\n", list);
    }
}