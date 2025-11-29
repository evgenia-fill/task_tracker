using System;
using System.Collections.Generic;

namespace SMMTracker.Domain.Entities;

public class User : Entity
{
    public long TelegramId { get; private set; }
    public string TelegramUsername { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string ProfileDescription { get; private set; }
    public string Hash { get; private set; }


    private readonly List<Invitation> invitations = new();
    private readonly List<UserTeam> userTeams = new();
    private readonly List<UserTask> userTasks = new();
    
    private User() {}

    public static User Create(long telegramId, string firstName, string lastName, string username)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new Exception();
        if (string.IsNullOrWhiteSpace(lastName))
            throw new Exception();

        var user = new User
        {
            TelegramId = telegramId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            TelegramUsername = username,
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