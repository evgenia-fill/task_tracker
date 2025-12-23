using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface ITaskRepository
{
    Task<SMMTracker.Domain.Entities.Task?> GetByIdAsync(int taskId);
    Task<SMMTracker.Domain.Entities.Task?> GetByIdWithEventAndTeamAsync(int taskId);

    Task<bool> ExistsAsync(int taskId);
    Task AddAsync(SMMTracker.Domain.Entities.Task task);
    Task UpdateStatusToReviewAsync(SMMTracker.Domain.Entities.Task task);
    Task UpdateStatusToDoneAsync(SMMTracker.Domain.Entities.Task task);
    Task UpdateStatusToInProgressAsync(SMMTracker.Domain.Entities.Task task);
    Task ChangeTaskNameAsync(SMMTracker.Domain.Entities.Task task, string name);
    Task ChangeTaskDescriptionAsync(SMMTracker.Domain.Entities.Task task, string name);
    Task SetTaskDeadlineAsync(SMMTracker.Domain.Entities.Task task, DateTime deadline);

    Task DeleteAsync(int taskId);
}