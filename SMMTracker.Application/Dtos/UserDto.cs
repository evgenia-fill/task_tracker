namespace SMMTracker.Application.Dtos;

public record UserDto
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string TelegramUsername { get; set; }
}