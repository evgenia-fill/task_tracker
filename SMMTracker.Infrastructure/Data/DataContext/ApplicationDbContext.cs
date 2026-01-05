using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Abstractions;
using SMMTracker.Domain.Entities;
using Task = SMMTracker.Domain.Entities.Task;

namespace SMMTracker.Infrastructure.Data.DataContext;

public class ApplicationDbContext : DbContext, IApplicationDbContext
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
    public DbSet<Calendar> Calendars { get; set; }
    
    // ДОБАВЛЕНЫ НОВЫЕ ТАБЛИЦЫ
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserAchievement> UserAchievements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка связи "Многие ко многим" или просто корректная связь
        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.UserAchievements)
            .HasForeignKey(ua => ua.UserId);

        modelBuilder.Entity<UserAchievement>()
            .HasOne(ua => ua.Achievement)
            .WithMany()
            .HasForeignKey(ua => ua.AchievementId);

        // Начальные данные (Seeding) - ОБЯЗАТЕЛЬНО, чтобы ачивки существовали в БД
        modelBuilder.Entity<Achievement>().HasData(
            new Achievement("Первые шаги", "Выполните свою первую задачу", "bi-check-circle", 1) { Id = 1 },
            new Achievement("На опыте", "Выполните 10 задач", "bi-star", 10) { Id = 2 },
            new Achievement("Трудяга", "Выполните 50 задач", "bi-gem", 50) { Id = 3 },
            new Achievement("Мастер", "Выполните 100 задач", "bi-trophy", 100) { Id = 4 },
            new Achievement("Элита", "Выполните 200 задач", "bi-crown", 200) { Id = 5 },
            new Achievement("Легенда", "Выполните 300 задач", "bi-rocket-takeoff", 300) { Id = 6 }
        );
    }
}