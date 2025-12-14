using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using Telegram.Bot.Requests.Abstractions;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Infrastructure.Repositories;

public class UserTaskRepository : IUserTaskRepository
{
    private readonly IApplicationDbContext _context;

    public UserTaskRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserTask?> GetUserTaskAsync(int taskId, int userId)
    {
        return await _context.UserTasks.FirstOrDefaultAsync(ut => ut.TaskId == taskId && ut.UserId == userId);
    }

    public async Task AddAsync(UserTask userTask)
    {
        await _context.UserTasks.AddAsync(userTask);
        await _context.SaveChangesAsync(default);
    }

    public async Task DeleteAsync(int userTaskId)
    {
        var userTask = await _context.UserTasks.FindAsync(userTaskId);
        if (userTask != null)
        {
            _context.UserTasks.Remove(userTask);
            await _context.SaveChangesAsync(default);
        }
    }
}