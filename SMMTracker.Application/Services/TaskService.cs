using SMMTracker.Application.Dtos;
using SMMTracker.Application.Interfaces;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.Enums;
using Task = SMMTracker.Domain.Entities.Task;

namespace SMMTracker.Application.Services;

using Microsoft.EntityFrameworkCore;

public class TaskService
{
    private readonly IApplicationDbContext _context;

    public TaskService(IApplicationDbContext context)
    {
        _context = context;
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
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync(cancellationToken);
        return task.Id;
    }

    public async System.Threading.Tasks.Task MoveTaskToReviewAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync([taskId], cancellationToken);
        if (task == null)
            throw new Exception("Task not found");
        task.MoveToReview();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task MoveTaskToDoneAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync([taskId], cancellationToken);
        if (task == null)
            throw new Exception("Task not found");
        task.MoveToDone();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task RemoveTaskAsync(int taskId,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync([taskId], cancellationToken);
        if (task == null)
            throw new Exception("Task not found");
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task ChangeTaskNameAsync(int taskId, string name,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync([taskId], cancellationToken);
        if (task == null)
            throw new Exception("Task not found");
        task.ChangeName(name);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task ChangeTaskDescriptionAsync(int taskId, string description,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync([taskId], cancellationToken);
        if (task == null)
            throw new Exception("Task not found");
        task.ChangeDescription(description);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async System.Threading.Tasks.Task SetTaskDeadlineAsync(int taskId, DateTime deadline,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync([taskId], cancellationToken);
        if (task == null)
            throw new Exception("Task not found");
        task.SetDeadline(deadline);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task AssignUserToTaskAsync(int taskId, int userIdToAssign, int adminId,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks
            .Include(t => t.Event)
            .ThenInclude(e => e.Team)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        if (task == null)
            throw new Exception("Task not found");

        var isAdmin = await _context
            .UserTeams
            .AnyAsync(ut => ut.TeamId == task.Event.Team.Id && ut.UserId == adminId && ut.Role == TeamRole.Admin,
                cancellationToken);
        if (!isAdmin)
            throw new UnauthorizedAccessException("Only admins can assign users to tasks");

        var userTask = new UserTask
        {
            TaskId = task.Id,
            UserId = userIdToAssign,
        };
        _context.UserTasks.Add(userTask);
        await _context.SaveChangesAsync(cancellationToken);
    }
}