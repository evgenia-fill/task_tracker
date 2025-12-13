using Microsoft.EntityFrameworkCore;
using SMMTracker.Infrastructure.Data.DataContext;

namespace SMMTracker.Tests;

public abstract class TestBase
{
    public static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        
        return context;
    }
}