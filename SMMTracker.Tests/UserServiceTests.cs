using System.Collections.Generic;
using FluentAssertions;
using Moq;
using SMMTracker.Application.Services;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Tests.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetUserProfileAsync_WhenUserExists_ReturnsProfile()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            Id = userId,
            FirstName = "Иван",
            LastName = "Иванов",
            UserName = "ivanov",
            ProfileDescription = "Тестовый пользователь"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetUserProfileAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Иван");
        result.LastName.Should().Be("Иванов");
        result.Description.Should().Be("Тестовый пользователь");

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserProfileAsync_WhenUserNotExists_ThrowsKeyNotFoundException()
    {
        // Arrange
        var userId = 999;
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            async () => await _userService.GetUserProfileAsync(userId));
    }

    [Fact]
    public async Task FindOrCreateUserAsync_WhenUserExists_ReturnsExistingUser()
    {
        // Arrange
        var telegramId = 123456789L;
        var existingUser = new User
        {
            Id = 1,
            TelegramId = telegramId,
            FirstName = "Существующий",
            LastName = "Пользователь",
            UserName = "existing_user"
        };

        var newUserClaim = new User
        {
            TelegramId = telegramId,
            FirstName = "Новый",
            LastName = "Пользователь"
        };

        _userRepositoryMock
            .Setup(r => r.GetByTelegramIdAsync(telegramId))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _userService.FindOrCreateUserAsync(newUserClaim);

        // Assert
        result.Id.Should().Be(1);
        result.FirstName.Should().Be("Существующий");
        result.LastName.Should().Be("Пользователь");

        _userRepositoryMock.Verify(r => r.GetByTelegramIdAsync(telegramId), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task FindOrCreateUserAsync_WhenUserNotExists_CreatesNewUser()
    {
        // Arrange
        var telegramId = 987654321L;
        var newUserClaim = new User
        {
            TelegramId = telegramId,
            FirstName = "Новый",
            LastName = "Пользователь",
            UserName = "new_user"
        };

        _userRepositoryMock
            .Setup(r => r.GetByTelegramIdAsync(telegramId))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.FindOrCreateUserAsync(newUserClaim);

        // Assert
        result.FirstName.Should().Be("Новый");
        result.LastName.Should().Be("Пользователь");

        _userRepositoryMock.Verify(r => r.GetByTelegramIdAsync(telegramId), Times.Once);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserProfileAsync_WhenUserExists_UpdatesUser()
    {
        // Arrange
        var userId = 1;
        var user = new User
        {
            Id = userId,
            FirstName = "Старое",
            LastName = "Имя",
            ProfileDescription = "Старое описание"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock
            .Setup(r => r.UpdateAsync(user))
            .Returns(Task.CompletedTask);

        var updateDto = new UserProfileDto
        {
            FirstName = "Новое",
            LastName = "Имя",
            Description = "Новое описание"
        };

        await _userService.UpdateUserProfileAsync(userId, updateDto);

        _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);

        _userRepositoryMock.Verify(r => r.UpdateAsync(It.Is<User>(u =>
            u.FirstName == "Новое" &&
            u.ProfileDescription == "Новое описание"
        )), Times.Once);
    }
}