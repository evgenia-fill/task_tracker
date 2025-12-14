using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Application.Abstractions;

public interface IUserService
{
   Task<UserDto> FindOrCreateUserAsync(User user);
   Task<UserProfileDto> GetUserProfileAsync(int userId);
   Task UpdateUserProfileAsync(int userId, string firstName, string lastName, string description);
}