using SMMTracker.Domain.Enums;

namespace SMMTracker.Application.Dtos;

public class TeamMemberDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public TeamRole Role { get; set; }
}