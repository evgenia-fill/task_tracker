using Microsoft.Data.Sqlite;

namespace Tg_Bot;

public class DatabaseService
{
    private readonly string connectionString;

    public DatabaseService()
    {
        var dbPath = Path.Combine(AppContext.BaseDirectory, "DataBase.db");
        connectionString = $"Data Source={dbPath}";
        Console.WriteLine($"База данных создаётся здесь: {dbPath}");
        Initialize();
    }

    private void Initialize()
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        
        var command = connection.CreateCommand();
        command.CommandText = 
            """
            CREATE TABLE IF NOT EXISTS Users (
                TelegramId INTEGER PRIMARY KEY,
                FullName TEXT NOT NULL,
                UniqueCode INTEGER NOT NULL 
            );
            """;
        command.ExecuteNonQuery();
    }
    
}