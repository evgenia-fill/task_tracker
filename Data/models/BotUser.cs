namespace Data.models;

public class BotUser
{
    public BotUser(string username, long telegramId, long uniqueCode)
    {
        UserName = username;
        TelegramId = telegramId;
        UniqueCode = uniqueCode;
    }
    
    public BotUser()
    {
    }

    public string UserName { get; set; } = string.Empty;
    public long TelegramId { get; set; }
    public long UniqueCode { get; set; }
}