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
using Infrastructure;


namespace Application;

public class MainBot: IBot
{
    private readonly ITelegramBotClient bot;
    private readonly List<IMessageHandler> messageHandlers;
    private readonly IStorageHandler reminderStorage;
    private readonly int maxRemindersCountSendPerSecond;

    public MainBot(ITelegramBotClient bot, IEnumerable<IMessageHandler> messageHandlers,
        IStorageHandler storage, int maxRemindersCountSendPerSecond)
    {
        this.bot = bot;
        this.messageHandlers = new List<IMessageHandler>(messageHandlers);
        this.reminderStorage = storage;
        this.maxRemindersCountSendPerSecond = maxRemindersCountSendPerSecond;
        if (maxRemindersCountSendPerSecond <= 0)
        {
            throw new ArgumentException($"{nameof(maxRemindersCountSendPerSecond)} must be positive");
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient,
        Update update, CancellationToken cancellationToken)
    {
        if (update.Type == UpdateType.Message)
        {
            var message = update.Message;
            if (message is null)
                return;

            var handlerArguments = new MessageHandlerArguments(botClient, reminderStorage, message);

            foreach (var handler in messageHandlers)
            {
                if (await handler.TryHandleMessageAsync(handlerArguments))
                    return;
            }

            await botClient.SendTextMessageAsync(message.Chat,
                "your message successfully bypassed all of our handlers; how`s that possible?!",
                cancellationToken: cancellationToken);
            throw new InvalidOperationException($"message {message.Text} escaped all handlers");
        }
    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine("exception recieved");
        Console.WriteLine(exception.Message);
    }

    private async Task StartSendingReminders()
    {
        while (true)
        {
            await SendReminderMessagesAsync();
            Thread.Sleep(1000);
        }
    }

    private async Task SendReminderMessagesAsync()
    {
        var reminders =  await reminderStorage.PopReminderDataRecordsAsync(maxCount: maxRemindersCountSendPerSecond);
        var reminderTasks = reminders
            .Select(reminder => SendReminderAsync(reminder.Payload));
        foreach (var task in reminderTasks)
        {
            await task;
        }
    }

    private async Task SendReminderAsync(ReminderData reminder)
    {
        await bot.SendTextMessageAsync(reminder.ChatId, reminder.TextContent);
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

        Task.Run(StartSendingReminders);

        reminderStorage.StartSavingCycle();

        if (forever)
        {
            Console.ReadLine();
        }
        else
        {
            Thread.Sleep(1000);
        }
    }
}
