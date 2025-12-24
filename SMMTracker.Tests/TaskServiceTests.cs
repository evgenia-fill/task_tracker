using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using FluentAssertions;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.Enums;
using SMMTracker.Domain.IRepositories;
using Xunit;
using TaskEntity = SMMTracker.Domain.Entities.Task;
using Task = System.Threading.Tasks.Task;

namespace SMMTracker.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock;
    private readonly Mock<IUserTeamRepository> _userTeamRepoMock;
    private readonly Mock<IUserTaskRepository> _userTaskRepoMock;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _taskRepoMock = new Mock<ITaskRepository>();
        _userTeamRepoMock = new Mock<IUserTeamRepository>();
        _userTaskRepoMock = new Mock<IUserTaskRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var eventRepoMock = new Mock<IEventRepository>();
        _service = new TaskService(_taskRepoMock.Object, 
            _userTeamRepoMock.Object, 
            _userTaskRepoMock.Object,
            unitOfWorkMock.Object,
            eventRepoMock.Object);
    }

    [Fact]
    public async Task CreateTaskAsync_ShouldCreateTask()
    {
        var dto = new CreateTaskDto { Name = "Task", Description = "D", EventId = 1 };
        _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<TaskEntity>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _service.CreateTaskAsync(dto);

        _taskRepoMock.Verify(r => r.AddAsync(It.IsAny<TaskEntity>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MoveTaskToReviewAsync_ShouldUpdateStatus()
    {
        var taskId = 1;
        var task = new TaskEntity("T", "D", 1, 1) { Id = taskId };
        _taskRepoMock.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        await _service.MoveTaskToReviewAsync(taskId);

        task.Status.Should().Be(SMMTracker.Domain.Enums.TaskStatus.InReview);
        _taskRepoMock.Verify(r => r.UpdateStatusToReviewAsync(task, 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveTaskAsync_ShouldDeleteTask()
    {
        var taskId = 1;
        await _service.DeleteTaskAsync(taskId);

        _taskRepoMock.Verify(r => r.DeleteAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ChangeTaskNameAsync_ShouldUpdateName()
    {
        var taskId = 1;
        var task = new TaskEntity("Old Name", "D", 1, 1) { Id = taskId };
        _taskRepoMock.Setup(r => r.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);

        await _service.ChangeTaskNameAsync(taskId, "New Name");

        task.Name.Should().Be("New Name");
        _taskRepoMock.Verify(r => r.UpdateStatusToInProgressAsync(task, 
            It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task AssignUserToTaskAsync_ShouldThrow_WhenNotAdmin()
    {
        var taskId = 1;
        var userId = 10;
        var teamId = 5;
        var calendarId = 1;

        var task = new TaskEntity("T", "D", 1, 1) { Id = taskId };
        var evt = new Event("E", "D", DateTime.Now, calendarId, 1, teamId);
        task.Event = evt;

        _taskRepoMock.Setup(r => r.GetByIdWithEventAndTeamAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(task);
        _userTeamRepoMock.Setup(r => r.IsUserAdminAsync(teamId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var action = async () => await _service.AssignUserToTaskAsync(taskId, 20, userId);

        await action.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}