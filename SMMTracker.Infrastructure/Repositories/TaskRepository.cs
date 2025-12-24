using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.IRepositories;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly IApplicationDbContext _context;

    public TaskRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SMMTracker.Domain.Entities.Task?> GetByIdAsync(int taskId, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken: cancellationToken);
    }
    
    public async Task<SMMTracker.Domain.Entities.Task?> GetByIdWithEventAndTeamAsync(int taskId, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks
            .Include(t => t.Event)
            .ThenInclude(e => e.Team)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken: cancellationToken);
    }

    public async Task<bool> ExistsAsync(int taskId, CancellationToken cancellationToken = default)
    {
        return await _context.Tasks.AnyAsync(t => t.Id == taskId, cancellationToken: cancellationToken);
    }

    public async Task AddAsync(Domain.Entities.Task task, CancellationToken cancellationToken = default)
    {
        await _context.Tasks.AddAsync(task, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatusToReviewAsync(Domain.Entities.Task task, CancellationToken cancellationToken = default)
    {
        task.MoveToReview();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatusToDoneAsync(Domain.Entities.Task task, CancellationToken cancellationToken = default)
    {
        task.MoveToDone();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateStatusToInProgressAsync(Domain.Entities.Task task, CancellationToken cancellationToken = default)
    {
        task.MoveToInProgress();
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeTaskNameAsync(Domain.Entities.Task task, string name, CancellationToken cancellationToken = default)
    {
        task.ChangeName(name);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangeTaskDescriptionAsync(Domain.Entities.Task task, string description, CancellationToken cancellationToken = default)
    {
        task.ChangeDescription(description);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task SetTaskDeadlineAsync(Domain.Entities.Task task, DateTime deadline, CancellationToken cancellationToken = default)
    {
        task.SetDeadline(deadline);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int taskId, CancellationToken cancellationToken = default)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}