using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using Task = SMMTracker.Domain.Entities.Task;

namespace SMMTracker.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserTeamRepository _userTeamRepository;
    private readonly IUserTaskRepository _userTaskRepository;

    public TaskService(ITaskRepository taskRepository, IUserTeamRepository userTeamRepository,
        IUserTaskRepository userTaskRepository)
    {
        _taskRepository = taskRepository;
        _userTeamRepository = userTeamRepository;
        _userTaskRepository = userTaskRepository;
    }

    public async Task<int> CreateTaskAsync(CreateTaskDto taskDto,
        CancellationToken cancellationToken = default)
    {
        var task = new Task(
            taskDto.Name,
            taskDto.Description,
            taskDto.EventId,
            taskDto.CalendarId
        );
        await _taskRepository.AddAsync(task);
        return task.Id;
    }

    public async System.Threading.Tasks.Task MoveTaskToReviewAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.UpdateStatusToReviewAsync(task);
    }

    public async System.Threading.Tasks.Task MoveTaskToDoneAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.UpdateStatusToDoneAsync(task);
    }

    public async System.Threading.Tasks.Task RemoveTaskAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.DeleteAsync(taskId);
    }

    public async System.Threading.Tasks.Task ChangeTaskNameAsync(int taskId, string name,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.ChangeTaskNameAsync(task, name);
    }

    public async System.Threading.Tasks.Task ChangeTaskDescriptionAsync(int taskId, string description,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.ChangeTaskDescriptionAsync(task, description);
    }

    public async System.Threading.Tasks.Task SetTaskDeadlineAsync(int taskId, DateTime deadline,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new Exception("Task not found");
    }

    public async System.Threading.Tasks.Task AssignUserToTaskAsync(int taskId, int userIdToAssign, int adminId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdWithEventAndTeamAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        if (!await _userTeamRepository.IsUserAdminAsync(task.Event.Team.Id, adminId))
            throw new UnauthorizedAccessException("Only admins can assign users to tasks");

        var userTask = new UserTask
        {
            TaskId = task.Id,
            UserId = userIdToAssign,
        };
        await _userTaskRepository.AddAsync(userTask);
    }
}