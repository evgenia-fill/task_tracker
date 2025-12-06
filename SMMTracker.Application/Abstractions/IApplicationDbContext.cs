using SMMTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = SMMTracker.Domain.Entities.Task;

namespace SMMTracker.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Task> Tasks { get; set; }
    DbSet<Team> Teams { get; set; }
    DbSet<Calendar> Calendars { get; set; }
    DbSet<Event> Events { get; set; }
    DbSet<UserTeam> UserTeams { get; set; }
    DbSet<UserTask> UserTasks { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}