using SMMTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Task = SMMTracker.Domain.Entities.Task;

namespace SMMTracker.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; set; }
    DbSet<Task> Tasks { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}