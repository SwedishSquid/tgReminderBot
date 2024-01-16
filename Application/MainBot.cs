using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Domain;


namespace Application;

public class MainBot
{
    private readonly ITelegramBotClient bot;
    private readonly List<IMessageHandler> messageHandlers;

    public MainBot(ITelegramBotClient bot, IEnumerable<IMessageHandler> messageHandlers)
    {
        this.bot = bot;
        this.messageHandlers = new List<IMessageHandler>(messageHandlers);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient,
        Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;
            if (message is null)
                return;
            
            foreach (var handler in messageHandlers)
            {
                if (handler.TryHandleMessage(message, botClient))
                    return;
            }

            await botClient.SendTextMessageAsync(message.Chat,
                "your message successfully bypassed all of our handlers; how`s that possible?!",
                cancellationToken: cancellationToken);
        }
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine("exception recieved");
        Console.WriteLine(exception.Message);
    }

    private async Task SendReminderMessage(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        var record = DataBaseHandler.FindClosestRecord();
        if (record == null) return;


        if ((record.Reminder.TimeToRemind - DateTime.Now).TotalSeconds > 1)
        {
            return;
        }

        await botClient.SendTextMessageAsync(new ChatId(record.Chat.Id), record.Reminder.text);
        DataBaseHandler.RemoveRecord(record);
    }

    public void Run(bool forever = false)
    {
        Console.WriteLine($"bot {bot.GetMeAsync().Result.FirstName} activated");

        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var recieverOptions = new ReceiverOptions()
        {
            AllowedUpdates = { }, //all update types
        };
        bot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            recieverOptions,
            cancellationToken
        );

        do
        {
            Thread.Sleep(1000);
            SendReminderMessage(bot, cancellationToken).Wait();
        } while (forever);

        Thread.Sleep(1000);
    }
}
