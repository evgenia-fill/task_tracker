using SMMTracker.Domain.Entities;

namespace SMMTracker.Domain.Entities;

public class Achievement : Entity
{
    public string Title { get; set; } // Например "Новичок"
    public string Description { get; set; } // "Выполнить 10 задач"
    public string IconClass { get; set; } // Класс иконки Bootstrap/FontAwesome, например "bi-star-fill"
    public int TasksThreshold { get; set; } // Порог задач: 1, 10, 50...

    public Achievement(string title, string description, string iconClass, int tasksThreshold)
    {
        Title = title;
        Description = description;
        IconClass = iconClass;
        TasksThreshold = tasksThreshold;
    }

    public Achievement() { }
}