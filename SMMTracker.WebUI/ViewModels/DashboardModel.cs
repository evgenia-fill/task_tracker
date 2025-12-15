using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMMTracker.WebUI.ViewModels
{
    public class DashboardModel : PageModel
    {
        public DashboardViewModel ViewModel { get; set; } = new DashboardViewModel();
        
        [BindProperty]
        public EditProfileViewModel EditProfile { get; set; } = new EditProfileViewModel();
        
        [BindProperty]
        public CreateTeamViewModel NewTeam { get; set; } = new CreateTeamViewModel();
        
        [BindProperty]
        public string InvitationCodeInput { get; set; } = "";
        
        // ДОБАВЛЯЕМ недостающие поля!
        public bool ShowProfileModal { get; set; }
        public bool ShowTeamModal { get; set; }
        
        public IActionResult OnGet()
        {
            LoadMockData();
            return Page();
        }
        
        public IActionResult OnPostUpdateProfile()
        {
            if (string.IsNullOrWhiteSpace(EditProfile.FirstName) || string.IsNullOrWhiteSpace(EditProfile.LastName))
            {
                TempData["ErrorMessage"] = "Имя и фамилия обязательны";
                LoadMockData();
                ShowProfileModal = true; // Устанавливаем флаг для показа модалки
                return Page();
            }
            
            ViewModel.UserInfo.FirstName = EditProfile.FirstName;
            ViewModel.UserInfo.LastName = EditProfile.LastName;
            ViewModel.UserInfo.ProfileDescription = EditProfile.ProfileDescription;
            
            TempData["SuccessMessage"] = "Профиль успешно обновлен!";
            return RedirectToPage();
        }
        
        public IActionResult OnPostJoinTeam()
        {
            if (string.IsNullOrWhiteSpace(InvitationCodeInput))
            {
                TempData["ErrorMessage"] = "Введите код приглашения";
                LoadMockData();
                return Page();
            }
            
            var newTeam = new TeamViewModel
            {
                Id = Guid.NewGuid(),
                Name = $"Команда {InvitationCodeInput}",
                Description = "Вы присоединились по приглашению",
                InvitationCode = InvitationCodeInput,
                CreatedAt = DateTime.Now,
                MemberCount = 3,
                IsOwner = false
            };
            
            ViewModel.Teams.Add(newTeam);
            TempData["SuccessMessage"] = "Вы успешно присоединились к команде!";
            
            return RedirectToPage();
        }
        
        public IActionResult OnPostCreateTeam()
        {
            if (string.IsNullOrWhiteSpace(NewTeam.Name))
            {
                TempData["ErrorMessage"] = "Название команды обязательно";
                LoadMockData();
                ShowTeamModal = true; // Устанавливаем флаг для показа модалки
                return Page();
            }
            
            var invitationCode = GenerateInvitationCode();
            
            var newTeam = new TeamViewModel
            {
                Id = Guid.NewGuid(),
                Name = NewTeam.Name,
                Description = NewTeam.Description,
                InvitationCode = invitationCode,
                CreatedAt = DateTime.Now,
                MemberCount = 1,
                IsOwner = true
            };
            
            ViewModel.Teams.Add(newTeam);
            TempData["SuccessMessage"] = $"Команда создана! Код приглашения: {invitationCode}";
            
            return RedirectToPage();
        }
        
        public IActionResult OnPostLeaveTeam(Guid teamId)
        {
            var team = ViewModel.Teams.FirstOrDefault(t => t.Id == teamId);
            if (team != null)
            {
                ViewModel.Teams.Remove(team);
                TempData["SuccessMessage"] = $"Вы покинули команду {team.Name}";
            }
            
            return RedirectToPage();
        }
        
        private void LoadMockData()
        {
            if (ViewModel.UserInfo == null || string.IsNullOrEmpty(ViewModel.UserInfo.FirstName))
            {
                ViewModel.UserInfo = new UserInfoViewModel
                {
                    TelegramId = 123456789,
                    FirstName = "Иван",
                    LastName = "Иванов",
                    TelegramUsername = "ivanov",
                    ProfileDescription = "Маркетолог, специалист по SMM. Работаю с Instagram, Telegram, VK."
                };
            }
            
            if (string.IsNullOrEmpty(EditProfile.FirstName))
            {
                EditProfile.FirstName = ViewModel.UserInfo.FirstName;
                EditProfile.LastName = ViewModel.UserInfo.LastName;
                EditProfile.ProfileDescription = ViewModel.UserInfo.ProfileDescription;
            }
            
            if (!ViewModel.Teams.Any())
            {
                ViewModel.Teams.AddRange(new List<TeamViewModel>
                {
                    new TeamViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Маркетинговая команда",
                        Description = "Работа над продвижением продукта в социальных сетях",
                        InvitationCode = "ABC123",
                        CreatedAt = DateTime.Now.AddDays(-30),
                        MemberCount = 5,
                        IsOwner = true
                    },
                    new TeamViewModel
                    {
                        Id = Guid.NewGuid(),
                        Name = "Проект 'Запуск'",
                        Description = "Подготовка к запуску нового продукта",
                        InvitationCode = "XYZ789",
                        CreatedAt = DateTime.Now.AddDays(-15),
                        MemberCount = 3,
                        IsOwner = false
                    }
                });
            }
        }
        
        private string GenerateInvitationCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}