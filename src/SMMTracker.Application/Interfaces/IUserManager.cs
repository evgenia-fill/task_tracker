using SMMTracker.Application.Dtos;

namespace SMMTracker.Application.Interfaces;

public interface IUserManager
{
   Task<UserDto> FindOrCreateUserAsync(long telegramId, string firstName, string lastName, string username);
   
}