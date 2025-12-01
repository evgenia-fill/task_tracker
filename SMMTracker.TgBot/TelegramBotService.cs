using SMMTracker.Application.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using SMMTracker.Domain.Entities;
using Task = System.Threading.Tasks.Task;
using User = SMMTracker.Domain.Entities.User;

namespace SMMTracker.TgBot;

public class TelegramBotService
{
    private readonly TelegramBotClient _client;
    private readonly IUserManager _userManager;

    public TelegramBotService(string token, IUserManager userManager)
    {
        _client = new TelegramBotClient(token);
        _userManager = userManager;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message == null)
            return;
        
        var chatId = update.Message.Chat.Id;
        var tgUser = update.Message.From;
        var user = new User
        {
            TelegramId = tgUser.Id,
            FirstName = tgUser.FirstName ?? "Unknown",
            LastName = tgUser.LastName ?? "",
            UserName = tgUser.Username ?? ""
        };
        var userDto = await _userManager.FindOrCreateUserAsync(user);

        await botClient.SendMessage(
            chatId: chatId,
            text: $"Добро пожаловать, {userDto.FirstName}! Вы успешно авторизованы.",
            cancellationToken: cancellationToken
        );
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            cancellationToken: cancellationToken
        );
        
        var me = await _client.GetMe();
        Console.WriteLine($"Бот @{me.Username} запущен!");
    }
}