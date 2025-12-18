using SMMTracker.Domain.Enums;

namespace SMMTracker.Application.Dtos;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsOwner { get; set; }
    public string InvitationCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public int MemberCount { get; set; }
}