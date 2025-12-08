namespace SMMTracker.Application.Dtos;

public class TelegramLoginDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Username { get; set; } = "";
    public long AuthDate { get; set; }
    public string Hash { get; set; } = "";
}