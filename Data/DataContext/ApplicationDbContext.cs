using Data.models;
using Microsoft.EntityFrameworkCore;
using Task = Data.models.Task;

namespace Data.DataContext;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<UserTeam> UserTeams { get; set; }
    public DbSet<UserTask> UserTasks { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<Calendar> Calendars { get; set; }
}