using SMMTracker.Application.Dtos;
using SMMTracker.Application.Interfaces;
using Task = SMMTracker.Domain.Entities.Task;

namespace SMMTracker.Application.Services;

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
}