namespace SMMTracker.Application.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public long TelegramId { get; set; }
    public string? ProfileDescription { get; set; }
}