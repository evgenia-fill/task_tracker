using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.Enums;
using SMMTracker.Domain.IRepositories;
using Task = SMMTracker.Domain.Entities.Task;
using TaskStatus = SMMTracker.Domain.Enums.TaskStatus;

namespace SMMTracker.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserTeamRepository _userTeamRepository;
    private readonly IUserTaskRepository _userTaskRepository;
    private readonly IEventRepository _eventRepository;

    public TaskService(ITaskRepository taskRepository, IUserTeamRepository userTeamRepository,
        IUserTaskRepository userTaskRepository, IEventRepository eventRepository)
    {
        _taskRepository = taskRepository;
        _userTeamRepository = userTeamRepository;
        _userTaskRepository = userTaskRepository;
        _eventRepository = eventRepository;
    }

    public async Task<int> CreateTaskAsync(CreateTaskDto dto,
        CancellationToken cancellationToken = default)
    {
        var parentEvent = await _eventRepository.GetByIdAsync(dto.EventId, cancellationToken);
        if (parentEvent == null)
            throw new InvalidOperationException($"Событие с Id={dto.EventId} не найдено");

        var task = new Task(
            dto.Name,
            dto.Description,
            dto.EventId,
            parentEvent.CalendarId
        )
        {
            Status = TaskStatus.InProgress
        };

        await _taskRepository.AddAsync(task, cancellationToken);

        return task.Id;
    }

    public async System.Threading.Tasks.Task MoveTaskToReviewAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.UpdateStatusToReviewAsync(task, cancellationToken);
    }

    public async System.Threading.Tasks.Task MoveTaskToDoneAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.UpdateStatusToDoneAsync(task, cancellationToken);
    }

    public async System.Threading.Tasks.Task MoveTaskToProgressAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.UpdateStatusToInProgressAsync(task, cancellationToken);
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.DeleteAsync(taskId, cancellationToken);
    }

    public async System.Threading.Tasks.Task ChangeTaskNameAsync(int taskId, string name,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.ChangeTaskNameAsync(task, name, cancellationToken);
    }

    public async System.Threading.Tasks.Task ChangeTaskDescriptionAsync(int taskId, string description,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.ChangeTaskDescriptionAsync(task, description, cancellationToken);
    }

    public async System.Threading.Tasks.Task SetTaskDeadlineAsync(int taskId, DateTime deadline,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
        if (task == null)
            throw new Exception("Task not found");
    }

    public async System.Threading.Tasks.Task AssignUserToTaskAsync(int taskId, int userIdToAssign, int adminId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdWithEventAndTeamAsync(taskId, cancellationToken);

        if (task == null)
            throw new Exception("Task not found");

        if (!await _userTeamRepository.IsUserAdminAsync(task.Event.Team.Id, adminId, cancellationToken))
            throw new UnauthorizedAccessException("Only admins can assign users to tasks");

        var userTask = new UserTask
        {
            TaskId = task.Id,
            UserId = userIdToAssign,
        };

        await _userTaskRepository.AddAsync(userTask, cancellationToken);
    }
}