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

    // public async Task<BotUser?> AddUserAsync(TelegramUser user)
    // {
    //     var existingUser = await GetUserByTgIdAsync(user.id);
    //     if (existingUser != null) return existingUser;
    //
    //     var fullName = $"{user.first_name} {user.last_name}".Trim();
    //     var code = GenerateUniqueCode();
    //
    //     await using var connection = new SqliteConnection(connectionString);
    //     await connection.OpenAsync();
    //
    //     var command = connection.CreateCommand();
    //     command.CommandText =
    //         "INSERT INTO Users (TelegramId, FullName, UniqueCode) VALUES (@id, @fullname, @uniquecode)";
    //     command.Parameters.AddWithValue("@id", user.id);
    //     command.Parameters.AddWithValue("@fullname", fullName);
    //     command.Parameters.AddWithValue("@uniquecode", code);
    //
    //     var rowsAffected = await command.ExecuteNonQueryAsync();
    //
    //     if (rowsAffected > 0)
    //     {
    //         return new BotUser
    //         {
    //             TelegramId = user.id,
    //             UserName = fullName,
    //             UniqueCode = code
    //         };
    //     }
    //
    //     return null;
    // }

    // public BotUser? GetUserByTgId(long tgId)
    // {
    //     using var connection = new SqliteConnection(connectionString);
    //     connection.Open();
    //     var command = connection.CreateCommand();
    //     command.CommandText =
    //         "SELECT TelegramId, FullName, UniqueCode FROM Users WHERE TelegramId = @tgId;";
    //     command.Parameters.AddWithValue("@tgId", tgId);
    //
    //     using var reader = command.ExecuteReader();
    //     if (reader.Read())
    //     {
    //         return new BotUser
    //         {
    //             TelegramId = Convert.ToInt64(reader["TelegramId"]),
    //             UserName = reader["FullName"].ToString(),
    //             UniqueCode = Convert.ToInt64(reader["UniqueCode"])
    //         };
    //     }
    //
    //     return null;
    // }

    // public async Task<BotUser?> GetUserByTgIdAsync(long tgId)
    // {
    //     await using var connection = new SqliteConnection(connectionString);
    //     await connection.OpenAsync();
    //
    //     var command = connection.CreateCommand();
    //     command.CommandText =
    //         "SELECT TelegramId, FullName, UniqueCode FROM Users WHERE TelegramId = @tgId;";
    //     command.Parameters.AddWithValue("@tgId", tgId);
    //
    //     await using var reader = await command.ExecuteReaderAsync();
    //
    //     if (await reader.ReadAsync())
    //     {
    //         return new BotUser
    //         {
    //             TelegramId = Convert.ToInt64(reader["TelegramId"]),
    //             UserName = reader["FullName"].ToString(),
    //             UniqueCode = Convert.ToInt64(reader["UniqueCode"])
    //         };
    //     }
    //
    //     return null;
    // }

    // public BotUser? GetUserByUniqueCode(long uniqueCode)
    // {
    //     using var connection = new SqliteConnection(connectionString);
    //     connection.Open();
    //     var command = connection.CreateCommand();
    //     command.CommandText =
    //         "SELECT TelegramId, FullName, UniqueCode FROM Users WHERE UniqueCode = @code;";
    //     command.Parameters.AddWithValue("@code", uniqueCode);
    //
    //     using var reader = command.ExecuteReader();
    //     if (reader.Read())
    //     {
    //         return new BotUser
    //         {
    //             TelegramId = Convert.ToInt64(reader["TelegramId"]),
    //             UserName = reader["FullName"].ToString(),
    //             UniqueCode = Convert.ToInt64(reader["UniqueCode"])
    //         };
    //     }
    //
    //     return null;
    // }

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

    //private int GenerateUniqueCode() => _random.Next(1000000, 9999999);
}