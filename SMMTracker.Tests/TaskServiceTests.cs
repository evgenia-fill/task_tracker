using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.Enums;
using Xunit;
using TaskEntity = SMMTracker.Domain.Entities.Task;

namespace SMMTracker.Tests;

public class TaskServiceTests : TestBase
{
    [Fact]
    public async void CreateTaskAsync_ShouldCreateTask()
    {
        await using var context = CreateContext();
        var service = new TaskService(context);
        var dto = new CreateTaskDto { Name = "Task", Description = "D", EventId = 1, CalendarId = 1 };

        var id = await service.CreateTaskAsync(dto);

        var task = await context.Tasks.FindAsync(id);
        task.Should().NotBeNull();
    }

    [Fact]
    public async void MoveTaskToReviewAsync_ShouldUpdateStatus()
    {
        await using var context = CreateContext();
        var service = new TaskService(context);
        var task = new TaskEntity("T", "D", 1, 1);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        await service.MoveTaskToReviewAsync(task.Id);

        var updated = await context.Tasks.FindAsync(task.Id);
        updated!.Status.Should().Be(Domain.Enums.TaskStatus.Review); 
    }

    [Fact]
    public async void RemoveTaskAsync_ShouldDeleteTask()
    {
        await using var context = CreateContext();
        var service = new TaskService(context);
        var task = new TaskEntity("T", "D", 1, 1);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        await service.RemoveTaskAsync(task.Id);

        var deleted = await context.Tasks.FindAsync(task.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async void ChangeTaskNameAsync_ShouldUpdateName()
    {
        await using var context = CreateContext();
        var service = new TaskService(context);
        var task = new TaskEntity("Old Name", "D", 1, 1);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        await service.ChangeTaskNameAsync(task.Id, "New Name");

        var updated = await context.Tasks.FindAsync(task.Id);
        updated!.Name.Should().Be("New Name");
    }

    [Fact]
    public async void AssignUserToTaskAsync_ShouldAssign_WhenAdmin()
    {
        await using var context = CreateContext();
        var service = new TaskService(context);

        var team = new Team("Team", "ABCDE");
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var evt = new Event("Evt", "D", DateTime.Now, 1);
        evt.Team = team; 
        context.Events.Add(evt);
        
        var task = new TaskEntity("T", "D", evt.Id, 1);
        task.Event = evt;
        context.Tasks.Add(task);

        var adminId = 10;
        var targetUserId = 20;

        context.UserTeams.Add(new UserTeam { TeamId = team.Id, UserId = adminId, Role = TeamRole.Admin });
        await context.SaveChangesAsync();

        await service.AssignUserToTaskAsync(task.Id, targetUserId, adminId);

        var userTask = await context.UserTasks.FirstOrDefaultAsync(ut => ut.TaskId == task.Id && ut.UserId == targetUserId);
        userTask.Should().NotBeNull();
    }

    [Fact]
    public async void AssignUserToTaskAsync_ShouldThrow_WhenNotAdmin()
    {
        await using var context = CreateContext();
        var service = new TaskService(context);

        var team = new Team("Team", "ABCDE");
        context.Teams.Add(team);
        
        var evt = new Event("Evt", "D", DateTime.Now, 1) { Team = team };
        context.Events.Add(evt);
        
        var task = new TaskEntity("T", "D", evt.Id, 1) { Event = evt };
        context.Tasks.Add(task);

        var regularUserId = 10;
        context.UserTeams.Add(new UserTeam { TeamId = team.Id, UserId = regularUserId, Role = TeamRole.User });
        await context.SaveChangesAsync();

        var action = async () => await service.AssignUserToTaskAsync(task.Id, 20, regularUserId);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}