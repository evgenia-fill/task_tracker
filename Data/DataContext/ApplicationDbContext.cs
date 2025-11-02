using Data.models;
using Microsoft.EntityFrameworkCore;

namespace Data.DataContext;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<BotUser> BotUsers { get; set; }
}