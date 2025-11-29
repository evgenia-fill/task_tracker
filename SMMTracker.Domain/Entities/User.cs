using System;
using System.Collections.Generic;

namespace SMMTracker.Domain.Entities;

public class User : Entity
{
    public long TelegramId { get; set; }
    public string TelegramUsername { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string ProfileDescription { get; set; }
    public string Hash { get; set; }


    private readonly List<Invitation> invitations = new();
    private readonly List<UserTeam> userTeams = new();
    private readonly List<UserTask> userTasks = new();

    public User() {}

    public static User Create(User otherUser)
    {
        if (string.IsNullOrWhiteSpace(otherUser.FirstName))
            throw new Exception();
        if (string.IsNullOrWhiteSpace(otherUser.LastName))
            throw new Exception();

        var user = new User
        {
            TelegramId = otherUser.TelegramId,
            FirstName = otherUser.FirstName.Trim(),
            LastName = otherUser.LastName.Trim(),
            TelegramUsername = otherUser.UserName.Trim(),
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

    public void AcceptInvitation(Invitation invitation)
    {
        
    }

    public void DeclineInvitation(Invitation invitation)
    {
        
    }
}