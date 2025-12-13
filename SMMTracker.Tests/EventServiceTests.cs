using FluentAssertions;
using SMMTracker.Application.Dtos;
using SMMTracker.Application.Services;
using SMMTracker.Domain.Entities;
using Xunit;
using TaskEntity = SMMTracker.Domain.Entities.Task;

namespace SMMTracker.Tests;

public class EventServiceTests : TestBase
{
    [Fact]
    public async void CreateEventAsync_ShouldCreateEvent()
    {
        await using var context = CreateContext();
        var service = new EventService(context);
        var dto = new CreateEventDto
        {
            Name = "New Event",
            Description = "Desc",
            Date = DateTime.Now,
            CalendarId = 1
        };

        var resultId = await service.CreateEventAsync(dto);

        var evt = await context.Events.FindAsync(resultId);
        evt.Should().NotBeNull();
        evt.Name.Should().Be("New Event");
    }

    [Fact]
    public async void GetEventsForMonthAsync_ShouldReturnEvents_ForSpecificMonth()
    {
        await using var context = CreateContext();
        var service = new EventService(context);
        const int calendarId = 10;
        var date = new DateTime(2023, 10, 15);

        context.Events.Add(new Event("Event 1", "Desc", date, calendarId));
        context.Events.Add(new Event("Event 2", "Desc", date.AddDays(1), calendarId));
        context.Events.Add(new Event("Other Month", "Desc", date.AddMonths(1), calendarId));
        context.Events.Add(new Event("Other Cal", "Desc", date, calendarId + 1));
        await context.SaveChangesAsync();

        var result = await service.GetEventsForMonthAsync(calendarId, 10, 2023);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(e => e.Date.Month == 10 && e.Date.Year == 2023);
    }

    [Fact]
    public async void GetEventDetailsAsync_ShouldReturnDetails_WithTasks()
    {
        await using var context = CreateContext();
        var service = new EventService(context);
        var evt = new Event("Main Event", "Full Desc", DateTime.Now, 1);
        context.Events.Add(evt);
        await context.SaveChangesAsync();

        var task = new TaskEntity("Task 1", "Desc", evt.Id, 1);
        context.Tasks.Add(task);
        await context.SaveChangesAsync();

        var result = await service.GetEventDetailsAsync(evt.Id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Main Event");
        result.Tasks.Should().HaveCount(1);
        result.Tasks.First().Name.Should().Be("Task 1");
    }

    [Fact]
    public async void GetEventDetailsAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var service = new EventService(context);

        var result = await service.GetEventDetailsAsync(999);

        result.Should().BeNull();
    }
}