using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application;

[MessageHandlerHelp("reminder", "creates a reminder", nameof(GetDetailedHelp))]
public class ReminderMessageHandler: IMessageHandler
{
    private static readonly string detailedHelp;
    private readonly IReminderMessageParser parser;

    static ReminderMessageHandler()
    {
        var reminderMessageFormats = Assembly.GetAssembly(typeof(IReminderMessageParser))
            .GetTypes()
            .Where(type => typeof(IReminderMessageParser).IsAssignableFrom(type) && !type.IsInterface)
            .Select(type => type.GetCustomAttribute<ReminderMessageFormatAttribute>())
            .Where(reminderMessageFormat => reminderMessageFormat is not null);
        detailedHelp = CreateDetailedHelp(reminderMessageFormats);
    }

    private static string CreateDetailedHelp(IEnumerable<ReminderMessageFormatAttribute> reminderMessageFormats)
    {
        var detailedHelpMessageBuilder = new StringBuilder("reminder formats:\n");
        foreach (var format in reminderMessageFormats)
            detailedHelpMessageBuilder.AppendLine($"\"{format.Pattern}\" ({format.Description})");
        return detailedHelpMessageBuilder.ToString();
    }

    public ReminderMessageHandler(IReminderMessageParser parser)
    {
        this.parser = parser;
    }

    public async Task<bool> TryHandleMessageAsync(IMessageHandlerArguments args)
    {
        var chatData = await args.StorageHandler.GetChatDataAsync(args.Message.Chat.Id);

        if (args.Message.Text is null 
            || !parser.TryParseReminderMessage(args.Message.Text, chatData, out var reminder))
        {
            return false;
        }

        await args.StorageHandler.AddReminderDataAsync(
            new ReminderData(reminder.TimeToRemind, args.Message.Chat.Id, reminder.text));

        await SendSuccessMessageToClient(args.Message.Chat, args.BotClient);

        return true;
    }

    private static async Task SendSuccessMessageToClient(Chat chat, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chat, "Done");
    }

    public static string GetDetailedHelp() => detailedHelp;
}
