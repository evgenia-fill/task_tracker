// using System;
// using System.Collections.Generic;
// using System.Threading;
// using Moq;
// using FluentAssertions;
// using SMMTracker.Application.Dtos;
// using SMMTracker.Application.Services;
// using SMMTracker.Domain.Entities;
// using SMMTracker.Domain.IRepositories;
// using SMMTracker.Infrastructure.Repositories;
// using Xunit;
// using TaskEntity = SMMTracker.Domain.Entities.Task;
// using Task = System.Threading.Tasks.Task;
//
// namespace SMMTracker.Tests;
//
// public class EventServiceTests
// {
//     private readonly Mock<IEventRepository> _eventRepoMock;
//     private readonly EventService _service;
//
//     public EventServiceTests()
//     {
//         _eventRepoMock = new Mock<IEventRepository>();
//         _service = new EventService(_eventRepoMock.Object);
//     }
//
//     [Fact]
//     public async Task CreateEventAsync_ShouldCreateEvent()
//     {
//         var dto = new CreateEventDto
//         {
//             Name = "New Event",
//             Description = "Desc",
//             Date = DateTime.Now
//         };
//
//         _eventRepoMock.Setup(r => 
//                 r.AddAsync(It.IsAny<Event>(), 
//                     It.IsAny<CancellationToken>()))
//             .Returns(Task.CompletedTask);
//         await _service.CreateEventAsync(dto);
//         _eventRepoMock.Verify(r => r.AddAsync(It.Is<Event>(e 
//             => e.Name == dto.Name), It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task GetEventsForMonthAsync_ShouldReturnEvents_ForSpecificMonth()
//     {
//         const int calendarId = 10;
//         var date = new DateTime(2023, 10, 15);
//         var events = new List<IEventRepository>
//         {
//             new EventRepository("Event 1", "Desc", date, calendarId, 1, 1),
//             new("Event 2", "Desc", date.AddDays(1), calendarId, 1, 1)
//         };
//
//         _eventRepoMock.Setup(r => r.GetByMonthAsync(calendarId, 10, 2023, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(events);
//
//         var result = await _service.GetEventsForMonthAsync(calendarId, 10, 2023);
//
//         result.Should().HaveCount(2);
//         _eventRepoMock.Verify(r => r.GetByMonthAsync(calendarId, 10, 2023, It.IsAny<CancellationToken>()), Times.Once);
//     }
//
//     [Fact]
//     public async Task GetEventDetailsAsync_ShouldReturnDetails_WithTasks()
//     {
//         var eventId = 1;
//         var evt = new Event("Main Event", "Full Desc", DateTime.Now, 1) { Id = eventId };
//         var task = new TaskEntity("Task 1", "Desc", eventId, 1);
//         evt.Tasks = new List<TaskEntity> { task };
//
//         _eventRepoMock.Setup(r => r.GetByIdWithTasksAsync(eventId, It.IsAny<CancellationToken>()))
//             .ReturnsAsync(evt);
//
//         var result = await _service.GetEventDetailsAsync(eventId);
//
//         result.Should().NotBeNull();
//         result!.Name.Should().Be("Main Event");
//         result.Tasks.Should().HaveCount(1);
//     }
//
//     [Fact]
//     public async Task GetEventDetailsAsync_ShouldReturnNull_WhenNotFound()
//     {
//         _eventRepoMock.Setup(r => r.GetByIdWithTasksAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
//             .ReturnsAsync((Event?)null);
//
//         var result = await _service.GetEventDetailsAsync(999);
//
//         result.Should().BeNull();
//     }
// }