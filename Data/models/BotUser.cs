namespace Data.models;

public class BotUser
{
    public string UserName { get; set; } = string.Empty;
    public long TelegramId { get; set; }
    public long UniqueCode { get; set; }
}