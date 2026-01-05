using SMMTracker.Application.Dtos;
using TaskEntity = SMMTracker.Domain.Entities.Task;

namespace SMMTracker.Application.Abstractions;

public interface ITaskService
{
    // Используем полные имена типов, чтобы не было конфликтов
    System.Threading.Tasks.Task<int> CreateTaskAsync(CreateTaskDto taskDto, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task MoveTaskToReviewAsync(int taskId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task MoveTaskToDoneAsync(int taskId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task DeleteTaskAsync(int taskId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task ChangeTaskNameAsync(int taskId, string name, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task ChangeTaskDescriptionAsync(int taskId, string description, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task SetTaskDeadlineAsync(int taskId, DateTime deadline, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task AssignUserToTaskAsync(int taskId, int userIdToAssign, int adminId, CancellationToken cancellationToken = default);
    System.Threading.Tasks.Task MoveTaskToProgressAsync(int taskId, CancellationToken cancellationToken = default);
}