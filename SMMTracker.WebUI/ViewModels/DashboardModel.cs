using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SMMTracker.Application.Abstractions;
using System.Security.Claims;
using SMMTracker.Application.Dtos;

namespace SMMTracker.WebUI.ViewModels;

public class DashboardModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ITeamService _teamService;

    public DashboardModel(IUserService userService, ITeamService teamService)
    {
        _userService = userService;
        _teamService = teamService;
    }

    public DashboardViewModel ViewModel { get; set; } = new();

    [BindProperty] public EditProfileViewModel EditProfile { get; set; } = new();

    [BindProperty] public CreateTeamViewModel NewTeam { get; set; } = new();

    [BindProperty] public string InvitationCodeInput { get; set; } = "";

    public bool ShowProfileModal { get; set; }
    public bool ShowTeamModal { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return RedirectToPage("/Login");
        }

        var userDto = await _userService.GetUserByIdAsync(userId);
        if (userDto == null)
        {
            return RedirectToPage("/Logout"); 
        }

        var teams = await _teamService.GetTeamsForUserAsync(userId);

        ViewModel.UserInfo = new UserInfoViewModel
        {
            Id = userDto.Id,
            TelegramId = userDto.TelegramId,
            FirstName = userDto.FirstName,
            LastName = userDto.LastName,
            TelegramUsername = userDto.UserName,
            ProfileDescription = userDto.ProfileDescription ?? "" 
        };

        ViewModel.Teams = teams.Select(t => new TeamViewModel
        {
            Id = t.Id, 
            Name = t.Name,
            InvitationCode = t.InvitationCode,
            CreatedAt = t.CreatedAt,
            MemberCount = t.MemberCount,
            IsOwner = t.IsOwner
        }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostJoinTeamAsync()
    {
        if (string.IsNullOrWhiteSpace(InvitationCodeInput))
        {
            TempData["ErrorMessage"] = "Введите код приглашения";
            return RedirectToPage();
        }

        try
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized();
            }

            var joinDto = new JoinTeamDto
            {
                Code = InvitationCodeInput,
                UserId = userId
            };

            var success = await _teamService.JoinTeamAsync(joinDto);

            if (success)
            {
                TempData["SuccessMessage"] = "Вы успешно присоединились к команде!";
            }
            else
            {
                TempData["ErrorMessage"] = "Неверный код приглашения или вы уже состоите в этой команде";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Произошла ошибка: {ex.Message}";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostLeaveTeamAsync(int teamId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        try
        {
            await _teamService.LeaveTeamAsync(teamId, userId);
            TempData["SuccessMessage"] = "Вы покинули команду";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ошибка: {ex.Message}";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostUpdateProfileAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            EditProfile.FirstName = ViewModel.UserInfo.FirstName;
            EditProfile.LastName = ViewModel.UserInfo.LastName;
            EditProfile.ProfileDescription = ViewModel.UserInfo.ProfileDescription;
            ShowProfileModal = true;
            return Page();
        }

        var profileDto = new UserProfileDto
        {
            FirstName = EditProfile.FirstName,
            LastName = EditProfile.LastName,
            Description = EditProfile.ProfileDescription
        };

        try
        {
            await _userService.UpdateUserProfileAsync(userId, profileDto);
            TempData["SuccessMessage"] = "Профиль успешно обновлен!";
            TempData["ShowProfileModal"] = false;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Ошибка при обновлении профиля: {ex.Message}";
            TempData["ShowProfileModal"] = true;
        }

        return RedirectToPage();
    }

    private string GenerateInvitationCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}