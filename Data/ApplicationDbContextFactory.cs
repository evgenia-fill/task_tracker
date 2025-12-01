using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Data.DataContext;
using System.IO;

namespace Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var solutionDir = Directory.GetParent(AppContext.BaseDirectory)
                              ?.Parent?.Parent?.Parent?.Parent?.FullName
                          ?? Directory.GetCurrentDirectory();

        var dataProjectDir = Path.Combine(solutionDir, "Data");
        var dbPath = Path.Combine(dataProjectDir, "DataBase.db");

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        return new ApplicationDbContext(options);
    }
}