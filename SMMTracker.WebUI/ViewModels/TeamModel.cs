using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SMMTracker.Application.Abstractions;
using SMMTracker.Application.Dtos;
using SMMTracker.WebUI.ViewModels;
using System.Security.Claims;

namespace SMMTracker.WebUI.ViewModels;

public class TeamModel : PageModel
{
    private readonly ITeamService _teamService;
    private readonly IEventService _eventService;
    private readonly IUserService _userService;

    public TeamViewModel Team { get; set; } = new();
    public List<TeamMemberViewModel> Members { get; set; } = new();
    public List<TeamEventViewModel> UpcomingEvents { get; set; } = new();
    public bool IsOwner { get; set; }

    // Делаем nullable, чтобы не ломалось при добавлении события
    [BindProperty] public string? NewMemberUsername { get; set; } 
    [BindProperty] public NewEventViewModel NewEvent { get; set; } = new();

    public TeamModel(ITeamService teamService, IEventService eventService, IUserService userService)
    {
        _teamService = teamService;
        _eventService = eventService;
        _userService = userService;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return RedirectToPage("/Login");
        }

        var teamDetails = await _teamService.GetTeamDetailsAsync(id);
        if (teamDetails == null)
        {
            return NotFound("Команда не найдена");
        }

        IsOwner = await _teamService.IsUserAdminAsync(id, userId);

        Team = new TeamViewModel
        {
            Id = teamDetails.Id,
            Name = teamDetails.Name,
            InvitationCode = teamDetails.InvitationCode,
            MemberCount = teamDetails.Members.Count,
            IsOwner = IsOwner
        };

        Members = teamDetails.Members.Select(m => new TeamMemberViewModel
        {
            Id = m.UserId,
            FirstName = m.FirstName,
            LastName = m.LastName,
            TelegramUsername = m.Username,
            Role = m.Role.ToString()
        }).ToList();

        var eventDtos = await _eventService.GetEventsForTeamAsync(id);
        UpcomingEvents = eventDtos.Select(e => new TeamEventViewModel
        {
            Id = e.Id,
            Title = e.Name,
            EventDate = e.Date
        }).ToList();

        return Page();
    }

    
    public async Task<IActionResult> OnPostAddEventAsync(int id)
    {
        // игнорируем NewMemberUsername, чтобы не ломало ModelState
        ModelState.Remove(nameof(NewMemberUsername));

        if (!ModelState.IsValid)
        {
            return await OnGetAsync(id);
        }

        var calendar = await _teamService.GetCalendarForTeamAsync(id);
        if (calendar == null)
        {
            TempData["ErrorMessage"] = "Календарь для команды не найден.";
            return RedirectToPage(new { id });
        }

        var createDto = new CreateEventDto
        {
            Name = NewEvent.Title,
            Description = NewEvent.Description,
            Date = NewEvent.EventDate,
            CalendarId = calendar.Id
        };

        await _eventService.CreateEventAsync(createDto);
        TempData["SuccessMessage"] = $"Мероприятие '{NewEvent.Title}' добавлено.";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveMemberAsync(int id, int memberId) // id - teamId, memberId - userId
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        try
        {
            await _teamService.RemoveUserFromTeamAsync(id, memberId, currentUserId);
            TempData["SuccessMessage"] = "Участник удален из команды.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
        }

        return RedirectToPage(new { id });
    }
}

public class TeamMemberViewModel
{
    public int Id { get; set; }
    public long TelegramId { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string TelegramUsername { get; set; } = "";
    public string Role { get; set; } = "";
    public DateTime JoinedAt { get; set; }
}

public class TeamEventViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime EventDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
}

public class NewEventViewModel
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime EventDate { get; set; } = DateTime.Now.AddDays(1);
}