using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Entities;

namespace SMMTracker.Application.Interfaces;

public interface IUserManager
{
   Task<UserDto> FindOrCreateUserAsync(User user);
   
}