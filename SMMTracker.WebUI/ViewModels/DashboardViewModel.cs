using System;
using System.Collections.Generic;

namespace SMMTracker.WebUI.ViewModels
{
    public class DashboardViewModel
    {
        public UserInfoViewModel UserInfo { get; set; } = new UserInfoViewModel();
        public List<TeamViewModel> Teams { get; set; } = new List<TeamViewModel>();
    }

    public class UserInfoViewModel
    {
        public long TelegramId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string TelegramUsername { get; set; } = "";
        public string ProfileDescription { get; set; } = "";
    }

    public class TeamViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string InvitationCode { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public int MemberCount { get; set; }
        public bool IsOwner { get; set; }
    }

    public class EditProfileViewModel
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string ProfileDescription { get; set; } = "";
    }

    public class CreateTeamViewModel
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }
}