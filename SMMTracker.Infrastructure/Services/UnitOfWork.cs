using SMMTracker.Application.Abstractions;
using SMMTracker.Infrastructure.Data.DataContext;

namespace SMMTracker.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork
{
    private ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}