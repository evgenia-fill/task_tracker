using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Data;
using Data.models;
using Task = System.Threading.Tasks.Task;
using User = Data.models.User;

namespace Tg_Bot;

public class TelegramBotService
{
    private readonly TelegramBotClient client;
    private readonly UserManager userManager;

    public TelegramBotService(string token, UserManager userManager)
    {
        client = new TelegramBotClient(token);
        this.userManager = userManager;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (update.Message == null)
            return;
        var tgId = update.Message.Chat.Id;
        var tgUser = new User
        {
            TelegramId = tgId,
            FirstName = update.Message.From?.FirstName ?? "Unknown",
            LastName = update.Message.From?.LastName ?? "",
            UserName = update.Message.From?.Username 
                       ?? update.Message.From?.FirstName 
                       ?? "user_" + tgId
        };

        await userManager.FindOrCreateUserAsync(tgUser);    
        await botClient.SendMessage(
            chatId: tgId,
            text: "Вы успешно авторизованы!",
            cancellationToken: cancellationToken
        );
    }

    private Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
        client.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            cancellationToken);
        var me = await client.GetMe();
    }
}