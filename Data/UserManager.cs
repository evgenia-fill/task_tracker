using Microsoft.Data.Sqlite;
using Data.models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Data.DataContext;

namespace Data;

public class UserManager
{
    private readonly string connectionString;
    private readonly ApplicationDbContext _context;
    private static readonly Random _random = new Random();

    public UserManager(string connectionString)
    {
        this.connectionString = connectionString;
    }
    public UserManager(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BotUser?> AddUserAsync(TelegramUser user)
    {
        var existingUser = await GetUserByTgIdAsync(user.id);
        if (existingUser != null) return existingUser;
        
        var fullName = $"{user.first_name} {user.last_name}".Trim();
        var code = GenerateUniqueCode();

        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText =
            "INSERT INTO Users (TelegramId, FullName, UniqueCode) VALUES (@id, @fullname, @uniquecode)";
        command.Parameters.AddWithValue("@id", user.id);
        command.Parameters.AddWithValue("@fullname", fullName);
        command.Parameters.AddWithValue("@uniquecode", code);

        var rowsAffected = await command.ExecuteNonQueryAsync();

        if (rowsAffected > 0)
        {
            return new BotUser
            {
                TelegramId = user.id,
                UserName = fullName,
                UniqueCode = code
            };
        }

        return null;
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
    
    public async Task<BotUser?> GetUserByTgIdAsync(long tgId)
    {
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(); 
    
        var command = connection.CreateCommand();
        command.CommandText =
            "SELECT TelegramId, FullName, UniqueCode FROM Users WHERE TelegramId = @tgId;";
        command.Parameters.AddWithValue("@tgId", tgId);

        await using var reader = await command.ExecuteReaderAsync(); 
    
        if (await reader.ReadAsync()) 
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

    public BotUser? GetUserByUniqueCode(long uniqueCode)
    {
        using var connection = new SqliteConnection(connectionString);
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText =
            "SELECT TelegramId, FullName, UniqueCode FROM Users WHERE UniqueCode = @code;";
        command.Parameters.AddWithValue("@code", uniqueCode);

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

    public async Task<BotUser> FindOrCreateUserAsync(long telegramId, string firstName, string? username)
    {
        var user = await _context.BotUsers.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
        if (user != null) return user;

        var newUsername = !string.IsNullOrEmpty(username) ? username : firstName;
        var newUniqueCode = GenerateUniqueCode();
        var newUser = new BotUser(newUsername, telegramId, newUniqueCode);

        await _context.BotUsers.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }
    
    public async Task<BotUser?> FindUserByTgIdAsync(long telegramId)
    {
        return await _context.BotUsers.FirstOrDefaultAsync(x => x.TelegramId == telegramId);
    }

    private int GenerateUniqueCode() => _random.Next(1000000, 9999999);
}