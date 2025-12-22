using System.ComponentModel.DataAnnotations;

namespace SMMTracker.WebUI.ViewModels;

public class NewEventViewModel
{
    [Required(ErrorMessage = "Введите название мероприятия")]
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    [Required(ErrorMessage = "Укажите дату и время мероприятия")]
    public DateTime EventDate { get; set; } = DateTime.Now.AddDays(1);
}
