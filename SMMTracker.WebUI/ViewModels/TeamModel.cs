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
    public int CalendarId { get; set; }

    [BindProperty] public string? NewMemberUsername { get; set; }
    [BindProperty] public NewEventViewModel NewEvent { get; set; } = new();

    [BindProperty] public string TeamName { get; set; } = "";
    [BindProperty] public string TeamDescription { get; set; } = "";
    [BindProperty] public int TeamId { get; set; }

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
            Description = teamDetails.Description,
            IsOwner = IsOwner
        };

        TeamId = Team.Id;
        TeamName = Team.Name;
        TeamDescription = Team.Description;

        Members = teamDetails.Members.Select(m => new TeamMemberViewModel
        {
            Id = m.UserId,
            FirstName = m.FirstName,
            LastName = m.LastName,
            TelegramUsername = m.Username,
            Role = m.Role.ToString()
        }).ToList();

        var eventDtos = await _eventService.GetEventsForTeamAsync(id);
        UpcomingEvents = eventDtos
            .Where(e => e.Date.ToLocalTime().Date >= DateTime.Today)
            .OrderBy(e => e.Date)
            .Select(e => new TeamEventViewModel
            {
                Id = e.Id,
                Title = e.Name,
                Description = e.Description,
                EventDate = e.Date,
                CreatedAt = e.CreatedAt,
                CreatedBy = e.CreatedBy
            }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostEditTeamAsync()
    {
        var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        if (!await _teamService.IsUserAdminAsync(TeamId, adminId))
        {
            TempData["ErrorMessage"] = "У вас нет прав для редактирования команды.";
            return RedirectToPage(new { id = TeamId });
        }

        if (string.IsNullOrWhiteSpace(TeamName))
        {
            TempData["ErrorMessage"] = "Название команды не может быть пустым";
            return Page();
        }

        try
        {
            await _teamService.UpdateTeamAsync(TeamId, TeamName, TeamDescription, adminId);
            TempData["SuccessMessage"] = "Данные команды обновлены";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
        }

        return RedirectToPage(new { id = TeamId });
    }

    public async Task<IActionResult> OnPostAddEventAsync(int id)
    {
        var isFormValid = true;
        if (string.IsNullOrWhiteSpace(NewEvent.Title))
        {
            TempData["ErrorMessage"] = "Название мероприятия не может быть пустым";
            isFormValid = false;
        }

        if (!isFormValid)
        {
            return await OnGetAsync(id);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        // var calendar = await _teamService.GetCalendarForTeamAsync(id);
        // if (calendar == null)
        // {
        //     TempData["ErrorMessage"] = "Календарь для команды не найден.";
        //     return RedirectToPage(new { id });
        // }

        var createDto = new CreateEventDto
        {
            Name = NewEvent.Title,
            Description = NewEvent.Description,
            Date = NewEvent.EventDate,
            TeamId = id,
            CreatedBy = userId
        };

        await _eventService.CreateEventAsync(createDto);
        TempData["SuccessMessage"] = $"Мероприятие '{NewEvent.Title}' добавлено";
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRemoveMemberAsync(int id, int memberId)
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