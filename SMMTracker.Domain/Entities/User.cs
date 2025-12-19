namespace SMMTracker.Domain.Entities;

public class User : Entity
{
    public long TelegramId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string? ProfileDescription { get; set; }
    public string Hash { get; set; }


    private readonly List<Invitation> Invitations = new();
    private readonly List<UserTeam> UserTeams = new();
    private readonly List<UserTask> UserTasks = new();

    public User() {}

    public static User Create(User otherUser)
    {
        if (string.IsNullOrWhiteSpace(otherUser.FirstName))
            throw new Exception();

        var user = new User
        {
            TelegramId = otherUser.TelegramId,
            FirstName = otherUser.FirstName.Trim(),
            LastName = otherUser.LastName.Trim(),
            Hash = Guid.NewGuid().ToString(),
            ProfileDescription = "",
            UserName = otherUser.UserName 
        };
        return user;
    }

    public void UpdateUserProfile(string firstName, string lastName, string description)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new Exception();
        if (string.IsNullOrWhiteSpace(lastName))
            throw new Exception();
        
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        ProfileDescription = description.Trim();
    }
}