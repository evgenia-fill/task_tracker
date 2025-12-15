using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace SMMTracker.WebUI.ViewModels
{
    public class TeamModel : PageModel
    {
        public TeamViewModel Team { get; set; } = new TeamViewModel();
        public List<TeamMemberViewModel> Members { get; set; } = new List<TeamMemberViewModel>();
        public List<TeamEventViewModel> UpcomingEvents { get; set; } = new List<TeamEventViewModel>();
        
        [BindProperty]
        public string NewMemberUsername { get; set; } = "";
        
        [BindProperty]
        public NewEventViewModel NewEvent { get; set; } = new NewEventViewModel();
        
        public bool IsOwner { get; set; }
        
        public IActionResult OnGet(Guid id)
        {
            // Загрузка тестовых данных
            LoadMockData(id);
            return Page();
        }
        
        public IActionResult OnPostAddMember(Guid id)
        {
            if (string.IsNullOrWhiteSpace(NewMemberUsername))
            {
                TempData["ErrorMessage"] = "Введите username пользователя";
                LoadMockData(id);
                return Page();
            }
            
            // Добавляем нового участника
            var newMember = new TeamMemberViewModel
            {
                Id = Guid.NewGuid(),
                TelegramId = new Random().Next(100000, 999999),
                FirstName = "Новый",
                LastName = "Участник",
                TelegramUsername = NewMemberUsername,
                Role = "Участник",
                JoinedAt = DateTime.Now
            };
            
            Members.Add(newMember);
            Team.MemberCount++;
            NewMemberUsername = "";
            
            TempData["SuccessMessage"] = $"Участник @{newMember.TelegramUsername} добавлен в команду";
            
            return RedirectToPage(new { id });
        }
        
        public IActionResult OnPostAddEvent(Guid id)
        {
            if (string.IsNullOrWhiteSpace(NewEvent.Title))
            {
                TempData["ErrorMessage"] = "Введите название мероприятия";
                LoadMockData(id);
                return Page();
            }
            
            var newEvent = new TeamEventViewModel
            {
                Id = Guid.NewGuid(),
                Title = NewEvent.Title,
                Description = NewEvent.Description,
                EventDate = NewEvent.EventDate,
                CreatedAt = DateTime.Now,
                CreatedBy = "Иван Иванов"
            };
            
            UpcomingEvents.Add(newEvent);
            NewEvent = new NewEventViewModel();
            
            TempData["SuccessMessage"] = $"Мероприятие '{newEvent.Title}' добавлено";
            
            return RedirectToPage(new { id });
        }
        
        public IActionResult OnPostRemoveMember(Guid id, Guid memberId)
        {
            var member = Members.FirstOrDefault(m => m.Id == memberId);
            if (member != null)
            {
                Members.Remove(member);
                Team.MemberCount--;
                TempData["SuccessMessage"] = $"Участник {member.FirstName} {member.LastName} удален";
            }
            
            return RedirectToPage(new { id });
        }
        
        private void LoadMockData(Guid teamId)
        {
            // Загружаем данные команды (в реальном приложении из базы данных)
            Team = new TeamViewModel
            {
                Id = teamId,
                Name = "Маркетинговая команда",
                Description = "Работа над продвижением продукта в социальных сетях",
                InvitationCode = "ABC123",
                CreatedAt = DateTime.Now.AddDays(-30),
                MemberCount = 5,
                IsOwner = true
            };
            
            IsOwner = Team.IsOwner;
            
            // Тестовые данные участников
            Members = new List<TeamMemberViewModel>
            {
                new TeamMemberViewModel
                {
                    Id = Guid.NewGuid(),
                    TelegramId = 123456789,
                    FirstName = "Иван",
                    LastName = "Иванов",
                    TelegramUsername = "ivanov",
                    Role = "Владелец",
                    JoinedAt = DateTime.Now.AddDays(-30)
                },
                new TeamMemberViewModel
                {
                    Id = Guid.NewGuid(),
                    TelegramId = 987654321,
                    FirstName = "Мария",
                    LastName = "Петрова",
                    TelegramUsername = "petrova",
                    Role = "Маркетолог",
                    JoinedAt = DateTime.Now.AddDays(-25)
                },
                new TeamMemberViewModel
                {
                    Id = Guid.NewGuid(),
                    TelegramId = 456123789,
                    FirstName = "Алексей",
                    LastName = "Сидоров",
                    TelegramUsername = "sidorov",
                    Role = "Контент-менеджер",
                    JoinedAt = DateTime.Now.AddDays(-20)
                },
                new TeamMemberViewModel
                {
                    Id = Guid.NewGuid(),
                    TelegramId = 789123456,
                    FirstName = "Екатерина",
                    LastName = "Кузнецова",
                    TelegramUsername = "kuznetsova",
                    Role = "Дизайнер",
                    JoinedAt = DateTime.Now.AddDays(-15)
                }
            };
            
            // Тестовые данные мероприятий
            UpcomingEvents = new List<TeamEventViewModel>
            {
                new TeamEventViewModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Планирование контента на месяц",
                    Description = "Обсуждение и планирование публикаций на февраль",
                    EventDate = DateTime.Now.AddDays(2),
                    CreatedAt = DateTime.Now.AddDays(-5),
                    CreatedBy = "Иван Иванов"
                },
                new TeamEventViewModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Анализ эффективности кампании",
                    Description = "Разбор результатов последней рекламной кампании",
                    EventDate = DateTime.Now.AddDays(5),
                    CreatedAt = DateTime.Now.AddDays(-3),
                    CreatedBy = "Мария Петрова"
                },
                new TeamEventViewModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Подготовка к запуску нового продукта",
                    Description = "Финальное обсуждение перед запуском",
                    EventDate = DateTime.Now.AddDays(7),
                    CreatedAt = DateTime.Now.AddDays(-1),
                    CreatedBy = "Алексей Сидоров"
                }
            };
        }
    }
    
    public class TeamMemberViewModel
    {
        public Guid Id { get; set; }
        public long TelegramId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string TelegramUsername { get; set; } = "";
        public string Role { get; set; } = "";
        public DateTime JoinedAt { get; set; }
    }
    
    public class TeamEventViewModel
    {
        public Guid Id { get; set; }
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
    
}