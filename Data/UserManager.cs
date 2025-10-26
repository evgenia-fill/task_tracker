using Microsoft.Data.Sqlite;
using Data.models; // Убедитесь, что для модели BotUser используется этот namespace

namespace Data;

public class UserManager
{
    private readonly string connectionString;

    // --- ИЗМЕНЕНИЕ ЗДЕСЬ ---
    // Конструктор теперь принимает готовую строку подключения, а не путь к файлу.
    public UserManager(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public void AddUser(long tgId, string fullname)
    {
        var existingUser = GetUserByTgId(tgId);
        if (existingUser != null)
        {
            Console.WriteLine($"Пользователь {fullname} (ID: {tgId}) уже существует в базе.");
            return;
        }

        var code = GenerateUniqueCode();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO Users (TelegramId, FullName, UniqueCode) VALUES (@id, @fullname, @uniquecode)";
        command.Parameters.AddWithValue("@id", tgId);
        command.Parameters.AddWithValue("@fullname", fullname);
        command.Parameters.AddWithValue("@uniquecode", code);

        command.ExecuteNonQuery();
        Console.WriteLine($"Пользователь {fullname} (ID: {tgId}) успешно добавлен в базу.");
    }

    public BotUser? GetUserByTgId(long tgId)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText =
            "SELECT TelegramId, FullName, UniqueCode FROM Users WHERE TelegramId = @tgId;";
        command.Parameters.AddWithValue("@tgId", tgId);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new BotUser
            {
                TelegramId = Convert.ToInt64(reader["TelegramId"]),
                UserName = reader["FullName"].ToString(),
                UniqueCode = Convert.ToInt64(reader["UniqueCode"])
            };
        }

        return null;
    }

    private int GenerateUniqueCode() => new Random().Next(1000000, 9999999);
}