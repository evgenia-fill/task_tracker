using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Interfaces;
using SMMTracker.Domain.Entities;
using SMMTracker.Infrastructure.Data.DataContext;

namespace SMMTracker.Infrastructure.Services;

public class UserManager : IUserManager
{
    private readonly ApplicationDbContext _context;

    public UserManager(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> FindOrCreateUserAsync(User otherUser)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.TelegramId == otherUser.TelegramId);

        if (user != null)
            return new UserDto
            {
                Id = user.Id,
                TelegramId = user.TelegramId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName
            };
        user = User.Create(otherUser);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return new UserDto
        {
            Id = user.Id, 
            TelegramId = user.TelegramId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.UserName
        };
    }
}