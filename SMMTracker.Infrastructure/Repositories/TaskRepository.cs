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

    public async Task<SMMTracker.Domain.Entities.Task?> GetByIdAsync(int taskId)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
    }
    
    public async Task<SMMTracker.Domain.Entities.Task?> GetByIdWithEventAndTeamAsync(int taskId)
    {
        return await _context.Tasks
            .Include(t => t.Event)
            .ThenInclude(e => e.Team)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task<bool> ExistsAsync(int taskId)
    {
        return await _context.Tasks.AnyAsync(t => t.Id == taskId);
    }

    public async Task AddAsync(Domain.Entities.Task task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync(default);
    }

    public async Task UpdateStatusToReviewAsync(Domain.Entities.Task task)
    {
        task.MoveToReview();
        await _context.SaveChangesAsync(default);
    }

    public async Task UpdateStatusToDoneAsync(Domain.Entities.Task task)
    {
        task.MoveToDone();
        await _context.SaveChangesAsync(default);
    }

    public async Task UpdateStatusToInProgressAsync(Domain.Entities.Task task)
    {
        task.MoveToInProgress();
        await _context.SaveChangesAsync(default);
    }

    public async Task ChangeTaskNameAsync(Domain.Entities.Task task, string name)
    {
        task.ChangeName(name);
        await _context.SaveChangesAsync(default);
    }

    public async Task ChangeTaskDescriptionAsync(Domain.Entities.Task task, string description)
    {
        task.ChangeDescription(description);
        await _context.SaveChangesAsync(default);
    }

    public async Task SetTaskDeadlineAsync(Domain.Entities.Task task, DateTime deadline)
    {
        task.SetDeadline(deadline);
        await _context.SaveChangesAsync(default);
    }

    public async Task DeleteAsync(int taskId)
    {
        var task = await _context.Tasks.FindAsync(taskId);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync(default);
        }
    }
}