using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Application.Abstractions;

public interface IUserService
{
    Task<UserDto> FindOrCreateUserAsync(User userClaim);
    Task<UserDto?> GetUserByIdAsync(int userId);
    Task<UserProfileDto> GetUserProfileAsync(int userId);
    Task UpdateUserProfileAsync(int userId, UserProfileDto dto);
    Task<UserDto?> GetUserByUsernameAsync(string username);
}