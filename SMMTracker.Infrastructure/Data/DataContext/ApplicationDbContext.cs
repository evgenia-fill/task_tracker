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
                new Achievement("Новичок", "Вступите в свою первую команду", "bi-people", 1) { Id = 1 },
                new Achievement("Командный игрок", "Вступите в 5 команд", "bi-people-fill", 5) { Id = 2 },
                new Achievement("Трудяга", "Вступите в 10 команд", "bi-person-check", 10) { Id = 3 },
                new Achievement("Душа компании", "Вступите в 20 команд", "bi-person-hearts", 20) { Id = 4 },
                new Achievement("Легенда сообщества", "Вступите в 50 команд", "bi-megaphone", 50) { Id = 5 },
                new Achievement("Друг Пьянзиной", "Вступите в 100 команд", "bi-award", 100) { Id = 6 }
            );
    }
}