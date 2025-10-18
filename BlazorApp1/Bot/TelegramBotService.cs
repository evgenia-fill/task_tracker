using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;

namespace BlazorApp1.Bot;

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
        var tgID = update.Message.Chat.Id;
        var text = update.Message.Text;
        var reference =
            "https://tr.pinterest.com/pin/35888128284853267/";

        if (text.StartsWith("/start"))
        {
            await botClient.SendMessage(tgID, "Привет! Введи свое имя и фамилию.",
                cancellationToken: cancellationToken);
        }
        else
        {
            userManager.AddUser(tgID, text);
            await botClient.SendMessage(tgID, $"Спасибо! Вот твоя персональная ссылка: {reference}",
                cancellationToken: cancellationToken);
        }
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
        Console.WriteLine($"Бот запущен: @{me.Username}");
    }
}