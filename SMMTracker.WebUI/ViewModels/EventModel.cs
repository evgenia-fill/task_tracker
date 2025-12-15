using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMMTracker.WebUI.ViewModels
{
    public class EventModel : PageModel
    {
        public EventViewModel Event { get; set; } = new EventViewModel();
        public TeamViewModel Team { get; set; } = new TeamViewModel();
        
        [BindProperty]
        public NewTaskViewModel NewTask { get; set; } = new NewTaskViewModel();
        
        [BindProperty]
        public string CommentText { get; set; } = "";
        
        public List<EventTaskViewModel> Tasks { get; set; } = new List<EventTaskViewModel>();
        public List<EventCommentViewModel> Comments { get; set; } = new List<EventCommentViewModel>();
        
        public bool IsOwner { get; set; }
        public bool IsTeamMember { get; set; }
        
        public IActionResult OnGet(Guid teamId, Guid eventId)
        {
            // Загрузка тестовых данных
            LoadMockData(teamId, eventId);
            return Page();
        }
        
        public IActionResult OnPostAddTask(Guid teamId, Guid eventId)
        {
            if (string.IsNullOrWhiteSpace(NewTask.Title))
            {
                TempData["ErrorMessage"] = "Введите название задачи";
                LoadMockData(teamId, eventId);
                return Page();
            }
            
            var newTask = new EventTaskViewModel
            {
                Id = Guid.NewGuid(),
                Title = NewTask.Title,
                Description = NewTask.Description,
                Status = TaskStatus.InProgress,
                CreatedBy = "Иван Иванов", // В реальном приложении текущий пользователь
                CreatedAt = DateTime.Now,
                Assignee = NewTask.Assignee
            };
            
            Tasks.Add(newTask);
            NewTask = new NewTaskViewModel();
            
            TempData["SuccessMessage"] = $"Задача '{newTask.Title}' добавлена в разработку";
            
            return RedirectToPage(new { teamId, eventId });
        }
        
        public IActionResult OnPostMoveToReview(Guid teamId, Guid eventId, Guid taskId)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null && task.Status == TaskStatus.InProgress)
            {
                task.Status = TaskStatus.InReview;
                task.MovedToReviewAt = DateTime.Now;
                TempData["SuccessMessage"] = $"Задача '{task.Title}' отправлена на ревью";
            }
            
            return RedirectToPage(new { teamId, eventId });
        }
        
        public IActionResult OnPostApproveTask(Guid teamId, Guid eventId, Guid taskId)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null && task.Status == TaskStatus.InReview && IsOwner)
            {
                task.Status = TaskStatus.Completed;
                task.CompletedAt = DateTime.Now;
                TempData["SuccessMessage"] = $"Задача '{task.Title}' одобрена и выполнена";
            }
            
            return RedirectToPage(new { teamId, eventId });
        }
        
        public IActionResult OnPostRejectTask(Guid teamId, Guid eventId, Guid taskId)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null && task.Status == TaskStatus.InReview && IsOwner)
            {
                task.Status = TaskStatus.InProgress;
                TempData["SuccessMessage"] = $"Задача '{task.Title}' возвращена в разработку";
            }
            
            return RedirectToPage(new { teamId, eventId });
        }
        
        public IActionResult OnPostAddComment(Guid teamId, Guid eventId)
        {
            if (string.IsNullOrWhiteSpace(CommentText))
            {
                TempData["ErrorMessage"] = "Введите текст комментария";
                LoadMockData(teamId, eventId);
                return Page();
            }
            
            var newComment = new EventCommentViewModel
            {
                Id = Guid.NewGuid(),
                Text = CommentText,
                CreatedBy = "Иван Иванов", // В реальном приложении текущий пользователь
                CreatedAt = DateTime.Now
            };
            
            Comments.Add(newComment);
            CommentText = "";
            
            return RedirectToPage(new { teamId, eventId });
        }
        
        public IActionResult OnPostDeleteTask(Guid teamId, Guid eventId, Guid taskId)
        {
            var task = Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                Tasks.Remove(task);
                TempData["SuccessMessage"] = $"Задача '{task.Title}' удалена";
            }
            
            return RedirectToPage(new { teamId, eventId });
        }
        
        private void LoadMockData(Guid teamId, Guid eventId)
        {
            // Тестовые данные команды
            Team = new TeamViewModel
            {
                Id = teamId,
                Name = "Маркетинговая команда",
                Description = "Работа над продвижением продукта в социальных сетях",
                CreatedAt = DateTime.Now.AddDays(-30),
                MemberCount = 5,
                IsOwner = true
            };
            
            IsOwner = Team.IsOwner;
            IsTeamMember = true; // В реальном приложении проверка, является ли пользователь участником команды
            
            // Тестовые данные мероприятия
            Event = new EventViewModel
            {
                Id = eventId,
                Title = "Планирование контента на месяц",
                Description = "Обсуждение и планирование публикаций на февраль. Необходимо подготовить контент-план, назначить ответственных и утвердить график публикаций.",
                EventDate = DateTime.Now.AddDays(2),
                CreatedAt = DateTime.Now.AddDays(-5),
                CreatedBy = "Иван Иванов",
                Status = EventStatus.Planned
            };
            
            // Тестовые данные задач
            Tasks = new List<EventTaskViewModel>
            {
                new EventTaskViewModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Подготовить контент-план для Instagram",
                    Description = "Разработать план публикаций на февраль для основного аккаунта",
                    Status = TaskStatus.InProgress,
                    CreatedBy = "Иван Иванов",
                    CreatedAt = DateTime.Now.AddDays(-4),
                    Assignee = "Мария Петрова"
                },
                new EventTaskViewModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Создать дизайн-макеты для Stories",
                    Description = "Подготовить 5 дизайнов для Stories с промо акцией",
                    Status = TaskStatus.InReview,
                    CreatedBy = "Мария Петрова",
                    CreatedAt = DateTime.Now.AddDays(-3),
                    Assignee = "Екатерина Кузнецова",
                    MovedToReviewAt = DateTime.Now.AddDays(-1)
                },
                new EventTaskViewModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Написать тексты для постов",
                    Description = "Подготовить текстовое наполнение для 10 постов",
                    Status = TaskStatus.Completed,
                    CreatedBy = "Алексей Сидоров",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    Assignee = "Алексей Сидоров",
                    CompletedAt = DateTime.Now
                },
                new EventTaskViewModel
                {
                    Id = Guid.NewGuid(),
                    Title = "Составить бюджет на рекламу",
                    Description = "Рассчитать бюджет для таргетированной рекламы в соцсетях",
                    Status = TaskStatus.InProgress,
                    CreatedBy = "Иван Иванов",
                    CreatedAt = DateTime.Now.AddDays(-1),
                    Assignee = "Иван Иванов"
                }
            };
            
            // Тестовые данные комментариев
            Comments = new List<EventCommentViewModel>
            {
                new EventCommentViewModel
                {
                    Id = Guid.NewGuid(),
                    Text = "Важно учесть праздничные дни в феврале при планировании контента",
                    CreatedBy = "Иван Иванов",
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new EventCommentViewModel
                {
                    Id = Guid.NewGuid(),
                    Text = "Готов предоставить статистику за январь для анализа",
                    CreatedBy = "Мария Петрова",
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new EventCommentViewModel
                {
                    Id = Guid.NewGuid(),
                    Text = "Дизайны будут готовы к концу недели",
                    CreatedBy = "Екатерина Кузнецова",
                    CreatedAt = DateTime.Now.AddDays(-1)
                }
            };
        }
    }
    
    public class EventViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime EventDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = "";
        public EventStatus Status { get; set; }
    }
    
    public class EventTaskViewModel
    {
        public Guid Id { get; set; }
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
}