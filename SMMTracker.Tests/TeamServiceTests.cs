using System;
using FluentAssertions;
using Moq;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.Enums;
using SMMTracker.Domain.IRepositories;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Tests;

public class TeamServiceTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IUserTeamRepository> _userTeamRepositoryMock;
    private readonly TeamService _teamService;

    public TeamServiceTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _userTeamRepositoryMock = new Mock<IUserTeamRepository>();
        _teamService = new TeamService(_teamRepositoryMock.Object, _userTeamRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateTeamAsync_ReturnsTeamId()
    {
        // Arrange
        var creatorId = 1;
        var dto = new CreateTeamDto { Name = "Тестовая команда" };
        var teamId = 5;

        _teamRepositoryMock
            .SetupSequence(r => r.ExistsByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(false);

        _teamRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Team>()))
            .Callback<Team>(team => team.Id = teamId)
            .Returns(Task.CompletedTask);

        _userTeamRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserTeam>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.CreateTeamAsync(dto, creatorId);

        // Assert
        result.Should().Be(teamId);
        _teamRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Team>()), Times.Once);
        _userTeamRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserTeam>()), Times.Once);
    }

    [Fact]
    public async Task CreateTeamAsync_GeneratesUniqueCode()
    {
        // Arrange
        var creatorId = 1;
        var dto = new CreateTeamDto { Name = "Команда" };

        _teamRepositoryMock
            .SetupSequence(r => r.ExistsByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(true)   // Первый код занят
            .ReturnsAsync(false); // Второй код свободен

        _teamRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Team>()))
            .Returns(Task.CompletedTask);

        _userTeamRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserTeam>()))
            .Returns(Task.CompletedTask);

        // Act
        await _teamService.CreateTeamAsync(dto, creatorId);

        // Assert
        _teamRepositoryMock.Verify(r => r.ExistsByCodeAsync(It.IsAny<string>()), Times.AtLeast(2));
    }

    [Fact]
    public async Task JoinTeamAsync_TeamExists_ReturnsTrue()
    {
        // Arrange
        var dto = new JoinTeamDto { Code = "ABC123", UserId = 2 };
        var team = new Team("Тестовая", "ABC123");
        team.Id = 1;

        _teamRepositoryMock
            .Setup(r => r.GetByCodeAsync("ABC123"))
            .ReturnsAsync(team);

        _userTeamRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<UserTeam>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.JoinTeamAsync(dto);

        // Assert
        result.Should().BeTrue();
        _userTeamRepositoryMock.Verify(r => r.AddAsync(It.Is<UserTeam>(ut => 
            ut.TeamId == 1 && 
            ut.UserId == 2 && 
            ut.Role == TeamRole.User)), Times.Once);
    }

    [Fact]
    public async Task JoinTeamAsync_TeamNotExists_ReturnsFalse()
    {
        // Arrange
        var dto = new JoinTeamDto { Code = "INVALID", UserId = 2 };

        _teamRepositoryMock
            .Setup(r => r.GetByCodeAsync("INVALID"))
            .ReturnsAsync((Team?)null);

        // Act
        var result = await _teamService.JoinTeamAsync(dto);

        // Assert
        result.Should().BeFalse();
        _userTeamRepositoryMock.Verify(r => r.AddAsync(It.IsAny<UserTeam>()), Times.Never);
    }

    [Fact]
    public async Task RemoveUserFromTeamAsync_AdminRemovesUser_Success()
    {
        // Arrange
        var teamId = 1;
        var userIdToRemove = 2;
        var adminId = 3;
    
        // Создаем userTeam для пользователя, которого удаляем (userIdToRemove = 2)
        var userTeamToRemove = new UserTeam { 
            Id = 10, 
            TeamId = teamId, 
            UserId = userIdToRemove  
        };

        _teamRepositoryMock
            .Setup(r => r.ExistsAsync(teamId))
            .ReturnsAsync(true);

        // Админ имеет права
        _userTeamRepositoryMock
            .Setup(r => r.IsUserAdminAsync(teamId, adminId))
            .ReturnsAsync(true);

        // Настраиваем возврат userTeam для пользователя, которого удаляем
        _userTeamRepositoryMock
            .Setup(r => r.GetUserTeamAsync(teamId, userIdToRemove))  //  userIdToRemove
            .ReturnsAsync(userTeamToRemove);

        // Act
        await _teamService.RemoveUserFromTeamAsync(teamId, userIdToRemove, adminId);

        // Assert
        _userTeamRepositoryMock.Verify(r => r.DeleteAsync(10), Times.Once);
    }

    [Fact]
    public async Task RemoveUserFromTeamAsync_NotAdmin_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var teamId = 1;
        var userIdToRemove = 2;
        var adminId = 3;

        _teamRepositoryMock
            .Setup(r => r.ExistsAsync(teamId))
            .ReturnsAsync(true);

        _userTeamRepositoryMock
            .Setup(r => r.IsUserAdminAsync(teamId, adminId))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _teamService.RemoveUserFromTeamAsync(teamId, userIdToRemove, adminId));
    }

    [Fact]
    public async Task RemoveUserFromTeamAsync_UserNotInTeam_ThrowsException()
    {
        // Arrange
        var teamId = 1;
        var userIdToRemove = 2;
        var adminId = 3;

        _teamRepositoryMock
            .Setup(r => r.ExistsAsync(teamId))
            .ReturnsAsync(true);

        _userTeamRepositoryMock
            .Setup(r => r.IsUserAdminAsync(teamId, adminId))
            .ReturnsAsync(true);

        _userTeamRepositoryMock
            .Setup(r => r.GetUserTeamAsync(teamId, userIdToRemove))
            .ReturnsAsync((UserTeam?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            async () => await _teamService.RemoveUserFromTeamAsync(teamId, userIdToRemove, adminId));
        
        exception.Message.Should().Be("User is not in the team");
    }

    [Fact]
    public async Task LeaveTeamAsync_UserIsAdmin_ThrowsException()
    {
        // Arrange
        var teamId = 1;
        var userId = 2;
        var userTeam = new UserTeam { Id = 10, TeamId = teamId, UserId = userId, Role = TeamRole.Admin };

        _userTeamRepositoryMock
            .Setup(r => r.GetUserTeamAsync(teamId, userId))
            .ReturnsAsync(userTeam);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            async () => await _teamService.LeaveTeamAsync(teamId, userId));
        
        exception.Message.Should().Be("Admin cannot leave team");
    }

    [Fact]
    public async Task LeaveTeamAsync_UserNotInTeam_ReturnsFalse()
    {
        // Arrange
        var teamId = 1;
        var userId = 2;

        _userTeamRepositoryMock
            .Setup(r => r.GetUserTeamAsync(teamId, userId))
            .ReturnsAsync((UserTeam?)null);

        // Act
        var result = await _teamService.LeaveTeamAsync(teamId, userId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task LeaveTeamAsync_RegularUser_Success()
    {
        // Arrange
        var teamId = 1;
        var userId = 2;
        var userTeam = new UserTeam { Id = 10, TeamId = teamId, UserId = userId, Role = TeamRole.User };

        _userTeamRepositoryMock
            .Setup(r => r.GetUserTeamAsync(teamId, userId))
            .ReturnsAsync(userTeam);

        // Act
        var result = await _teamService.LeaveTeamAsync(teamId, userId);

        // Assert
        result.Should().BeTrue();
        _userTeamRepositoryMock.Verify(r => r.DeleteAsync(10), Times.Once);
    }
}