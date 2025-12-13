using FluentAssertions;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;
using SMMTracker.Domain.Entities;
using Xunit;

namespace SMMTracker.Tests;

public class CalendarServiceTests
{
    [Fact]
    public async void CreateCalendarAsync_ShouldCreateCalendar_WhenTeamExists()
    {
        await using var context = TestBase.CreateContext();
        var service = new CalendarService(context);
        var team = new Team("Test Team", "CODE1");
        context.Teams.Add(team);
        await context.SaveChangesAsync();

        var dto = new CreateCalendarDto { TeamId = team.Id };

        var resultId = await service.CreateCalendarAsync(dto);

        var calendar = await context.Calendars.FindAsync(resultId);
        calendar.Should().NotBeNull();
        calendar.TeamId.Should().Be(team.Id);
    }

    [Fact]
    public async void CreateCalendarAsync_ShouldThrowException_WhenTeamDoesNotExist()
    {
        await using var context = TestBase.CreateContext();
        var service = new CalendarService(context);
        var dto = new CreateCalendarDto { TeamId = 999 };

        var action = async () => await service.CreateCalendarAsync(dto);

        await action.Should().ThrowAsync<Exception>()
            .WithMessage("Команда с Id=999 не найдена.");
    }
}