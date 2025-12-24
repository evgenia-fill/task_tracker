using System;
using System.Threading;
using Moq;
using FluentAssertions;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;
using SMMTracker.Domain.Entities;
using SMMTracker.Domain.IRepositories;
using Xunit;

namespace SMMTracker.Tests;

public class CalendarServiceTests
{
    private readonly Mock<ICalendarRepository> _calendarRepoMock;
    private readonly Mock<ITeamRepository> _teamRepoMock;
    private readonly CalendarService _service;

    public CalendarServiceTests()
    {
        _calendarRepoMock = new Mock<ICalendarRepository>();
        _teamRepoMock = new Mock<ITeamRepository>();
        _service = new CalendarService(_calendarRepoMock.Object, _teamRepoMock.Object);
    }

    [Fact]
    public async void CreateCalendarAsync_ShouldCreateCalendar_WhenTeamExists()
    {
        var teamId = 1;
        var dto = new CreateCalendarDto { TeamId = teamId };
        _teamRepoMock.Setup(r => r.ExistsAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        await _service.CreateCalendarAsync(dto);
        _calendarRepoMock.Verify(r => 
            r.AddAsync(It.Is<Calendar>(c => c.TeamId == teamId), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async void CreateCalendarAsync_ShouldThrowException_WhenTeamDoesNotExist()
    {
        var teamId = 999;
        var dto = new CreateCalendarDto { TeamId = teamId };
        
        _teamRepoMock.Setup(r => 
                r.ExistsAsync(teamId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        var action = async () => await _service.CreateCalendarAsync(dto);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage($"Команда с Id={teamId} не найдена.");
        _calendarRepoMock.Verify(r => 
            r.AddAsync(It.IsAny<Calendar>(), It.IsAny<CancellationToken>()), 
            Times.Never);
    }
}