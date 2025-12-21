using System;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SMMTracker.Domain.Entities;
using SMMTracker.Infrastructure.Data.DataContext;
using SMMTracker.Infrastructure.Repositories;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Tests.IntegrationTests;

public class UserRepositoryTests : IAsyncLifetime
{
    private ApplicationDbContext _context;
    private UserRepository _repository;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_User_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);
        await _context.Database.EnsureCreatedAsync();
        _repository = new UserRepository(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_AddsUserToDatabase()
    {
        var user = new User
        {
            TelegramId = 123456789,
            FirstName = "Тест",
            LastName = "Юзер",              
            UserName = "testuser",           
            Hash = Guid.NewGuid().ToString() 
        };
        
        await _repository.AddAsync(user);
        
        var savedUser = await _context.Users.FirstOrDefaultAsync();
        savedUser.Should().NotBeNull();
        savedUser!.FirstName.Should().Be("Тест");
        savedUser.LastName.Should().Be("Юзер");     
        savedUser.TelegramId.Should().Be(123456789);
        savedUser.UserName.Should().Be("testuser");   
        savedUser.Hash.Should().NotBeNullOrEmpty();   
        savedUser.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser()
    {
        var user = new User
        {
            TelegramId = 111222333,
            FirstName = "Анна",
            LastName = "Петрова",           
            UserName = "annapetrova",        
            Hash = Guid.NewGuid().ToString() 
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var result = await _repository.GetByIdAsync(user.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.FirstName.Should().Be("Анна");
        result.LastName.Should().Be("Петрова");      
        result.UserName.Should().Be("annapetrova");   
        result.Hash.Should().Be(user.Hash);         
    }

    [Fact]
    public async Task GetByTelegramIdAsync_ReturnsCorrectUser()
    {
        var telegramId = 999888777L;
        var user = new User
        {
            TelegramId = telegramId,
            FirstName = "Поиск",
            LastName = "Телеграм",          
            UserName = "telegramuser",       
            Hash = Guid.NewGuid().ToString() 
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        var result = await _repository.GetByTelegramIdAsync(telegramId);

        result.Should().NotBeNull();
        result!.TelegramId.Should().Be(telegramId);
        result.FirstName.Should().Be("Поиск");
        result.LastName.Should().Be("Телеграм");    
        result.UserName.Should().Be("telegramuser");  
        result.Hash.Should().Be(user.Hash);           
    }

    [Fact]
    public async Task UpdateAsync_UpdatesUser()
    {
        var user = new User
        {
            TelegramId = 555555555,
            FirstName = "Старое",
            LastName = "Имя",               
            UserName = "olduser",            
            Hash = Guid.NewGuid().ToString() 
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.FirstName = "Новое";
        await _repository.UpdateAsync(user);
        
        var updatedUser = await _context.Users.FindAsync(user.Id);
        updatedUser!.FirstName.Should().Be("Новое");
        updatedUser.LastName.Should().Be("Имя");       
        updatedUser.UserName.Should().Be("olduser");    
        updatedUser.Hash.Should().Be(user.Hash);        
    }

    [Fact]
    public async Task DeleteAsync_RemovesUser()
    {
        var user = new User
        {
            TelegramId = 777777777,
            FirstName = "Удалить",
            LastName = "Меня",               
            UserName = "deleteuser",         
            Hash = Guid.NewGuid().ToString() 
        };
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await _repository.DeleteAsync(user.Id);
        var deletedUser = await _context.Users.FindAsync(user.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueForExistingUser()
    {
        var user = new User 
        { 
            TelegramId = 888888888, 
            FirstName = "Существует",
            LastName = "Пользователь",      
            UserName = "existinguser",       
            Hash = Guid.NewGuid().ToString() 
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var result = await _repository.ExistsAsync(user.Id);
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalseForNonExistingUser()
    {
        var result = await _repository.ExistsAsync(999999);
        result.Should().BeFalse();
    }
}