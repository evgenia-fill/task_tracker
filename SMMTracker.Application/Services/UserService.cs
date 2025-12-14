using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;

    public UserService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> FindOrCreateUserAsync(User otherUser)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.TelegramId == otherUser.TelegramId);

        if (user != null)
            return GetUserDto(user);

        user = User.Create(otherUser);
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync(default);

        return GetUserDto(user);
    }

    private static UserDto GetUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.UserName
        };
    }

    public async Task<UserProfileDto> GetUserProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"Пользователь Id{userId} не найден");
        return new UserProfileDto()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Description = user.ProfileDescription ?? ""
        };
    }


    public async Task UpdateUserProfileAsync(int userId, string firstName, string lastName, string description)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new KeyNotFoundException($"Пользователь Id{userId} не найден");

        user.UpdateUserProfile(firstName, lastName, description);

        await _context.SaveChangesAsync(default);
    }
}