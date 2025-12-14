using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Repositories;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Tests;

public class TeamRepositoryTests : IAsyncLifetime
{
    private ApplicationDbContext _context;
    private TeamRepository _repository;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_Team_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        await _context.Database.EnsureCreatedAsync();
        _repository = new TeamRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_AddsTeamWithCode()
    {
        // Arrange
        var team = new Team("Тестовая команда", "TEST123");

        // Act
        await _repository.AddAsync(team);

        // Assert
        var savedTeam = await _context.Teams.FirstOrDefaultAsync();
        savedTeam.Should().NotBeNull();
        savedTeam!.Name.Should().Be("Тестовая команда");
        savedTeam.Code.Should().Be("TEST123");
    }

    [Fact]
    public async Task GetByCodeAsync_ReturnsCorrectTeam()
    {
        // Arrange
        var team1 = new Team("Первая", "CODE111");
        var team2 = new Team("Вторая", "CODE222");
        
        _context.Teams.AddRange(team1, team2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCodeAsync("CODE222");

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Вторая");
        result.Code.Should().Be("CODE222");
    }

    [Fact]
    public async Task ExistsByCodeAsync_ReturnsCorrectValues()
    {
        // Arrange
        var team = new Team("Команда", "EXISTS123");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act & Assert
        (await _repository.ExistsByCodeAsync("EXISTS123")).Should().BeTrue();
        (await _repository.ExistsByCodeAsync("NOTEXIST")).Should().BeFalse();
    }
    
    [Fact]
    public async Task DeleteAsync_RemovesTeam()
    {
        // Arrange
        var team = new Team("Удалить", "DEL123");
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(team.Id);

        // Assert
        var deletedTeam = await _context.Teams.FindAsync(team.Id);
        deletedTeam.Should().BeNull();
    }
}