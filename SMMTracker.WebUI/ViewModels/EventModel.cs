using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;

namespace SMMTracker.WebUI.ViewModels;

public class EventModel : PageModel
{
    public EventViewModel Event { get; set; } = new();
    public TeamViewModel Team { get; set; } = new();

    [BindProperty] public NewTaskViewModel NewTask { get; set; } = new();

    [BindProperty] public string CommentText { get; set; } = "";

    public List<EventTaskViewModel> Tasks { get; set; } = new();
    public List<EventCommentViewModel> Comments { get; set; } = new();

    public bool IsOwner { get; set; }
    public bool IsTeamMember { get; set; }
    private readonly IEventService _eventService;
    private readonly ITaskService _taskService;
    private readonly ITeamService _teamService;

    public EventModel(IEventService eventService, ITaskService taskService, ITeamService teamService)
    {
        _eventService = eventService;
        _taskService = taskService;
        _teamService = teamService;
    }

    public async Task<IActionResult> OnGetAsync(int calendarId, int eventId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return RedirectToPage("/Login");
        }

        var eventDetails = await _eventService.GetEventDetailsAsync(eventId);
        if (eventDetails == null)
        {
            return NotFound("Событие не найдено");
        }

        IsOwner = await _teamService.IsUserAdminAsync(eventDetails.TeamId, userId);

        Event = new EventViewModel
        {
            Id = eventDetails.Id,
            Title = eventDetails.Name,
            Description = eventDetails.Description,
            EventDate = eventDetails.Date,
            // Status = (ViewModels.EventStatus)eventDetails.Status
        };

        Tasks = eventDetails.Tasks.Select(t => new EventTaskViewModel
        {
            Id = t.Id,
            Title = t.Name,
            Status = (ViewModels.TaskStatus)t.Status,
        }).ToList();
        
        return Page(); 
    }

    public async Task<IActionResult> OnPostAddTaskAsync(int calendarId, int eventId)
    {
        if (!ModelState.IsValid)
        {
            return await OnGetAsync(calendarId, eventId);
        }

        var eventDetails = await _eventService.GetEventDetailsAsync(eventId);
        if (eventDetails == null) return NotFound();

        var createTaskDto = new CreateTaskDto
        {
            Name = NewTask.Title,
            Description = NewTask.Description,
            EventId = eventId,
            CalendarId = calendarId
        };

        await _taskService.CreateTaskAsync(createTaskDto);

        TempData["SuccessMessage"] = $"Задача '{NewTask.Title}' добавлена";
        return RedirectToPage(new { calendarId, eventId });
    }

    public async Task<IActionResult> OnPostMoveToReviewAsync(int eventId, int taskId)
    {
        await _taskService.MoveTaskToReviewAsync(taskId);
        TempData["SuccessMessage"] = "Задача отправлена на ревью";
        return RedirectToPage(new { eventId });
    }

    public async Task<IActionResult> OnPostMoveToDoneAsync(int eventId, int taskId)
    {
        await _taskService.MoveTaskToDoneAsync(taskId);
        TempData["SuccessMessage"] = "Задача выполнена";
        return RedirectToPage(new { eventId });
    }

    public async Task<IActionResult> OnPostRejectTaskAsync(int eventId, int taskId)
    {
        await _taskService.MoveTaskToProgressAsync(taskId);
        TempData["SuccessMessage"] = "Задача возвращена в разработку";
        return RedirectToPage(new { eventId });
    }

    public async Task<IActionResult> OnPostDeleteTaskAsync(int eventId, int taskId)
    {
        try
        {
            await _taskService.DeleteTaskAsync(taskId);
            TempData["SuccessMessage"] = "Задача удалена.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
        }

        return RedirectToPage(new { eventId });
    }

    public async Task<IActionResult> OnPostAddCommentAsync(int calendarId, int eventId)
    {
        if (string.IsNullOrWhiteSpace(CommentText))
        {
            TempData["ErrorMessage"] = "Введите текст комментария";
            return await OnGetAsync(calendarId, eventId);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        
        TempData["SuccessMessage"] = "Комментарий добавлен.";
        return RedirectToPage(new { eventId });
    }
}

public class EventViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime EventDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public EventStatus Status { get; set; }
}

public class EventTaskViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public TaskStatus Status { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string Assignee { get; set; } = "";
    public DateTime? MovedToReviewAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class EventCommentViewModel
{
    public Guid Id { get; set; }
    public string Text { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class NewTaskViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Assignee { get; set; } = "";
}

public enum TaskStatus
{
    InProgress,
    InReview,
    Completed
}

public enum EventStatus
{
    Planned,
    InProgress,
    Completed,
    Cancelled
}