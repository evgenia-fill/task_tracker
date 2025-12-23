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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventRepository _eventRepository;

    public TaskService(ITaskRepository taskRepository, IUserTeamRepository userTeamRepository,
        IUserTaskRepository userTaskRepository, IUnitOfWork unitOfWork, IEventRepository eventRepository)
    {
        _taskRepository = taskRepository;
        _userTeamRepository = userTeamRepository;
        _userTaskRepository = userTaskRepository;
        _unitOfWork = unitOfWork;
        _eventRepository = eventRepository;
    }

    public async Task<int> CreateTaskAsync(CreateTaskDto taskDto,
        CancellationToken cancellationToken = default)
    {
        var parentEvent = await _eventRepository.GetByIdAsync(taskDto.EventId);

        if (parentEvent == null)
            throw new InvalidOperationException($"Событие с Id={taskDto.EventId} не найдено");

        var task = new Task(
            taskDto.Name,
            taskDto.Description,
            taskDto.EventId,
            parentEvent.CalendarId
        );
        await _taskRepository.AddAsync(task);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
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

    public async System.Threading.Tasks.Task MoveTaskToProgressAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null)
            throw new Exception("Task not found");

        await _taskRepository.UpdateStatusToInProgressAsync(task);
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(int taskId,
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

    public async Task<int> AddTaskWithAssigneeAsync(CreateTaskDto dto,
        CancellationToken cancellationToken = default)
    {
        var parentEvent = await _eventRepository.GetByIdAsync(dto.EventId);
        if (parentEvent == null)
            throw new InvalidOperationException($"Событие с Id={dto.EventId} не найдено");

        var task = new Task(
            dto.Name,
            dto.Description,
            dto.EventId,
            parentEvent.CalendarId
        );

        task.Status = TaskStatus.InProgress;

        await _taskRepository.AddAsync(task);

        var userTask = new UserTask
        {
            TaskId = task.Id,
            UserId = dto.AssignedUserId,
            Role = UserTaskRole.Executor
        };

        await _userTaskRepository.AddAsync(userTask);

        return task.Id;
    }
}