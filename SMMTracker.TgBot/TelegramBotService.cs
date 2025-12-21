using SMMTracker.Application.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;
using User = SMMTracker.Domain.Entities.User;

namespace SMMTracker.TgBot;

public class TelegramBotService
{
    private readonly TelegramBotClient _client;
    private readonly IUserService _userService;

    public TelegramBotService(string token, IUserService userService)
    {
        _client = new TelegramBotClient(token);
        _userService = userService;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message == null)
            return;

        var chatId = update.Message.Chat.Id;
        var tgUser = update.Message.From;

        if (tgUser == null)
            return;

        var messageText = update.Message.Text ?? string.Empty;

        if (messageText.Equals("/start", StringComparison.OrdinalIgnoreCase))
        {
            await _client.SendMessage(
                chatId: chatId,
                text: "Привет! Я бот для авторизации в SMM Tracker.\n\n" +
                      "Напишите любое сообщение для регистрации в системе.", cancellationToken: cancellationToken);
            return;
        }

        try
        {
            var user = new User
            {
                TelegramId = tgUser.Id,
                FirstName = tgUser.FirstName,
                LastName = tgUser.LastName ?? "",
                UserName = tgUser.Username ?? ""
            };

            var userDto = await _userService.FindOrCreateUserAsync(user);

            await _client.SendMessage(
                chatId: chatId,
                text: $"Добро пожаловать, {userDto.FirstName}! Вы успешно авторизованы.\n",
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка обработки сообщения: {ex.Message}");

            try
            {
                await _client.SendMessage(
                    chatId: chatId,
                    text: "Произошла ошибка при обработке вашего запроса.", cancellationToken: cancellationToken);
            }
            catch
            {
                // ignored
            }
        }
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            cancellationToken: cancellationToken
        );

        var me = await _client.GetMe(cancellationToken: cancellationToken);
    }
}