using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class UserTaskRepository : IUserTaskRepository
{
    private readonly IApplicationDbContext _context;

    public UserTaskRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserTask?> GetUserTaskAsync(int taskId, int userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserTasks.FirstOrDefaultAsync(ut => ut.TaskId == taskId && ut.UserId == userId, cancellationToken: cancellationToken);
    }

    public async Task AddAsync(UserTask userTask, CancellationToken cancellationToken = default)
    {
        await _context.UserTasks.AddAsync(userTask, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int userTaskId, CancellationToken cancellationToken = default)
    {
        var userTask = await _context.UserTasks.FindAsync(userTaskId);
        if (userTask != null)
        {
            _context.UserTasks.Remove(userTask);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}