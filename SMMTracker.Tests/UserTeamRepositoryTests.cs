using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.Enums;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Repositories;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Tests.IntegrationTests;

public class UserTeamRepositoryTests : IAsyncLifetime
{
    private ApplicationDbContext _context;
    private UserTeamRepository _repository;
    private User _user1;
    private User _user2;
    private Team _team1;
    private Team _team2;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_UserTeam_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        await _context.Database.EnsureCreatedAsync();
        
        _repository = new UserTeamRepository(_context);
        
        _user1 = new User 
        { 
            TelegramId = 111, 
            FirstName = "Пользователь1",
            LastName = "Тестовый1",          
            UserName = "user1test",          
            Hash = Guid.NewGuid().ToString() 
        };
        _user2 = new User 
        { 
            TelegramId = 222, 
            FirstName = "Пользователь2",
            LastName = "Тестовый2",          
            UserName = "user2test",          
            Hash = Guid.NewGuid().ToString() 
        };
        _team1 = new Team("Команда1", "TEAM1");
        _team2 = new Team("Команда2", "TEAM2");
        
        _context.Users.AddRange(_user1, _user2);
        _context.Teams.AddRange(_team1, _team2);
        await _context.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_CreatesUserTeamRelationship()
    {
        var userTeam = new UserTeam
        {
            UserId = _user1.Id,
            TeamId = _team1.Id,
            Role = TeamRole.Admin
        };

        await _repository.AddAsync(userTeam);
        var saved = await _context.UserTeams.FirstOrDefaultAsync();
        saved.Should().NotBeNull();
        saved!.UserId.Should().Be(_user1.Id);
        saved.TeamId.Should().Be(_team1.Id);
        saved.Role.Should().Be(TeamRole.Admin);
    }

    [Fact]
    public async Task GetUserTeamAsync_ReturnsRelationship()
    {
        var userTeam = new UserTeam
        {
            UserId = _user1.Id,
            TeamId = _team1.Id,
            Role = TeamRole.User
        };
        _context.UserTeams.Add(userTeam);
        await _context.SaveChangesAsync();
        var result = await _repository.GetUserTeamAsync(_team1.Id, _user1.Id);
        
        result.Should().NotBeNull();
        result!.UserId.Should().Be(_user1.Id);
        result.TeamId.Should().Be(_team1.Id);
    }

    [Fact]
    public async Task IsUserAdminAsync_ReturnsCorrectValues()
    {
        var adminUserTeam = new UserTeam
        {
            UserId = _user1.Id,
            TeamId = _team1.Id,
            Role = TeamRole.Admin
        };
        
        var regularUserTeam = new UserTeam
        {
            UserId = _user2.Id,
            TeamId = _team1.Id,
            Role = TeamRole.User
        };
        
        _context.UserTeams.AddRange(adminUserTeam, regularUserTeam);
        await _context.SaveChangesAsync();

        (await _repository.IsUserAdminAsync(_team1.Id, _user1.Id)).Should().BeTrue();
        (await _repository.IsUserAdminAsync(_team1.Id, _user2.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_RemovesUserTeam()
    {
        var userTeam = new UserTeam
        {
            UserId = _user1.Id,
            TeamId = _team1.Id,
            Role = TeamRole.Admin
        };
        _context.UserTeams.Add(userTeam);
        await _context.SaveChangesAsync();
        await _repository.DeleteAsync(userTeam.Id);
        
        var deleted = await _context.UserTeams.FindAsync(userTeam.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsCorrectValues()
    {
        var userTeam = new UserTeam
        {
            UserId = _user1.Id,
            TeamId = _team1.Id,
            Role = TeamRole.Admin
        };
        _context.UserTeams.Add(userTeam);
        await _context.SaveChangesAsync();
        
        (await _repository.ExistsAsync(userTeam.Id)).Should().BeTrue();
        (await _repository.ExistsAsync(999999)).Should().BeFalse();
    }
}