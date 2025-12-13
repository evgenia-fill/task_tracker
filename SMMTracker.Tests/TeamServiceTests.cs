using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.Enums;
using Xunit;

namespace SMMTracker.Tests;

public class TeamServiceTests : TestBase
{
    [Fact]
    public async void CreateTeamAsync_ShouldCreateTeamAndAdmin()
    {
        await using var context = CreateContext();
        var service = new TeamService(context);
        const int creatorId = 1;
        var dto = new CreateTeamDto { Name = "Alpha" };

        var teamId = await service.CreateTeamAsync(dto, creatorId);

        var team = await context.Teams.FindAsync(teamId);
        team.Should().NotBeNull();
        team!.Code.Should().HaveLength(5);

        var userTeam = await context.UserTeams.FirstOrDefaultAsync(ut => ut.TeamId == teamId && ut.UserId == creatorId);
        userTeam.Should().NotBeNull();
        userTeam!.Role.Should().Be(TeamRole.Admin);
    }

    [Fact]
    public async void JoinTeamAsync_ShouldAddUser_WhenCodeValid()
    {
        await using var context = CreateContext();
        var service = new TeamService(context);
        var team = new Team("Alpha", "XYZ12");
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var dto = new JoinTeamDto { Code = "XYZ12", UserId = 2 };

        var result = await service.JoinTeamAsync(dto);

        result.Should().BeTrue();
        var userTeam = await context.UserTeams.FirstOrDefaultAsync(ut => ut.TeamId == team.Id && ut.UserId == 2);
        userTeam.Should().NotBeNull();
        userTeam.Role.Should().Be(TeamRole.User);
    }

    [Fact]
    public async void JoinTeamAsync_ShouldReturnFalse_WhenCodeInvalid()
    {
        await using var context = CreateContext();
        var service = new TeamService(context);
        var dto = new JoinTeamDto { Code = "WRONG", UserId = 2 };

        var result = await service.JoinTeamAsync(dto);

        result.Should().BeFalse();
    }

    [Fact]
    public async void RemoveUserFromTeamAsync_ShouldRemove_WhenRequesterIsAdmin()
    {
        await using var context = CreateContext();
        var service = new TeamService(context);
        var teamId = 1;
        var adminId = 10;
        var userId = 20;

        context.Teams.Add(new Team("T", "C") { Id = teamId });
        context.UserTeams.Add(new UserTeam { TeamId = teamId, UserId = adminId, Role = TeamRole.Admin });
        context.UserTeams.Add(new UserTeam { TeamId = teamId, UserId = userId, Role = TeamRole.User });
        await context.SaveChangesAsync();

        await service.RemoveUserFromTeamAsync(teamId, userId, adminId);

        var removedUser = await context.UserTeams.FirstOrDefaultAsync(ut => ut.TeamId == teamId && ut.UserId == userId);
        removedUser.Should().BeNull();
    }

    [Fact]
    public async void RemoveUserFromTeamAsync_ShouldThrow_WhenRequesterNotAdmin()
    {
        await using var context = CreateContext();
        var service = new TeamService(context);
        var teamId = 1;
        var user1 = 10;
        var user2 = 20;

        context.Teams.Add(new Team("T", "C") { Id = teamId });
        context.UserTeams.Add(new UserTeam { TeamId = teamId, UserId = user1, Role = TeamRole.User });
        await context.SaveChangesAsync();

        var action = async () => await service.RemoveUserFromTeamAsync(teamId, user2, user1);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async void LeaveTeamAsync_ShouldRemoveUser_WhenNotAdmin()
    {
        await using var context = CreateContext();
        var service = new TeamService(context);
        var teamId = 1;
        var userId = 10;

        context.UserTeams.Add(new UserTeam { TeamId = teamId, UserId = userId, Role = TeamRole.User });
        await context.SaveChangesAsync();

        var result = await service.LeaveTeamAsync(teamId, userId);

        result.Should().BeTrue();
        var record = await context.UserTeams.FirstOrDefaultAsync(ut => ut.TeamId == teamId && ut.UserId == userId);
        record.Should().BeNull();
    }

    [Fact]
    public async void LeaveTeamAsync_ShouldThrow_WhenUserIsAdmin()
    {
        await using var context = CreateContext();
        var service = new TeamService(context);
        var teamId = 1;
        var adminId = 10;

        context.UserTeams.Add(new UserTeam { TeamId = teamId, UserId = adminId, Role = TeamRole.Admin });
        await context.SaveChangesAsync();

        var action = async () => await service.LeaveTeamAsync(teamId, adminId);

        await action.Should().ThrowAsync<Exception>().WithMessage("Admin cannot leave team");
    }
}