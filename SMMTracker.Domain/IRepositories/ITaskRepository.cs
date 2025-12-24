using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Domain.IRepositories;

public interface ITaskRepository
{
    Task<SMMTracker.Domain.Entities.Task?> GetByIdAsync(int taskId, CancellationToken cancellationToken = default);

    Task<SMMTracker.Domain.Entities.Task?> GetByIdWithEventAndTeamAsync(int taskId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(int taskId, CancellationToken cancellationToken = default);
    Task AddAsync(SMMTracker.Domain.Entities.Task task, CancellationToken cancellationToken = default);
    Task UpdateStatusToReviewAsync(SMMTracker.Domain.Entities.Task task, CancellationToken cancellationToken = default);
    Task UpdateStatusToDoneAsync(SMMTracker.Domain.Entities.Task task, CancellationToken cancellationToken = default);

    Task UpdateStatusToInProgressAsync(SMMTracker.Domain.Entities.Task task,
        CancellationToken cancellationToken = default);

    Task ChangeTaskNameAsync(SMMTracker.Domain.Entities.Task task, string name,
        CancellationToken cancellationToken = default);

    Task ChangeTaskDescriptionAsync(SMMTracker.Domain.Entities.Task task, string name,
        CancellationToken cancellationToken = default);

    Task SetTaskDeadlineAsync(SMMTracker.Domain.Entities.Task task, DateTime deadline,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(int taskId, CancellationToken cancellationToken = default);
}