using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositoryes;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> FindOrCreateUserAsync(User userClaim)
    {
        var user = await _userRepository.GetByTelegramIdAsync(userClaim.TelegramId);

        if (user != null)
            return GetUserDto(user);

        user = User.Create(userClaim);
        await _userRepository.AddAsync(user);

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
        var user = await _userRepository.GetByIdAsync(userId);
        
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
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            throw new KeyNotFoundException($"Пользователь Id{userId} не найден");
        
        user.UpdateUserProfile(firstName, lastName, description);
        
        await _userRepository.UpdateAsync(user);
    }
}