using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface IUserTaskRepository
{
    Task<UserTask?> GetUserTaskAsync(int taskId, int userId);
    Task AddAsync(UserTask userTask);
    Task DeleteAsync(int userTaskId);
}