namespace SMMTracker.WebUI.ViewModels;

public class DashboardViewModel
{
    public UserInfoViewModel UserInfo { get; set; } = new UserInfoViewModel();
    public List<TeamViewModel> Teams { get; set; } = new List<TeamViewModel>();
}


