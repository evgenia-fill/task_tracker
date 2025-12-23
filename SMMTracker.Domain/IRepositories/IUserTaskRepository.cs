using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface IUserTaskRepository
{
    Task AddAsync(UserTask userTask, CancellationToken cancellationToken = default);
    Task DeleteAsync(int userTaskId, CancellationToken cancellationToken = default);
}