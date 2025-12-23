namespace SMMTracker.Application.Dtos;

public class TeamDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string InvitationCode { get; set; }
    public string Description { get; set; }
    public List<TeamMemberDto> Members { get; set; } = new();
}