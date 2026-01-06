using SMMTracker.Application.Dtos;

namespace SMMTracker.Application.Abstractions;

public interface ITaskService
{
    Task<int> CreateTaskAsync(CreateTaskDto taskDto, CancellationToken cancellationToken = default);
    Task MoveTaskToReviewAsync(int taskId, CancellationToken cancellationToken = default);
    Task MoveTaskToDoneAsync(int taskId, CancellationToken cancellationToken = default);

    Task DeleteTaskAsync(int taskId, CancellationToken cancellationToken = default);

    Task ChangeTaskNameAsync(int taskId, string name, CancellationToken cancellationToken = default);

    Task ChangeTaskDescriptionAsync(int taskId, string description, CancellationToken cancellationToken = default);

    Task SetTaskDeadlineAsync(int taskId, DateTime deadline, CancellationToken cancellationToken = default);

    Task AssignUserToTaskAsync(int taskId, int userIdToAssign, int adminId,
        CancellationToken cancellationToken = default);

    Task MoveTaskToProgressAsync(int taskId, CancellationToken cancellationToken = default);
}