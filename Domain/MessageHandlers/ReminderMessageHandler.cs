using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Domain;

[MessageHandlerHelp("reminder", "creates a reminder")]
public class ReminderMessageHandler: IMessageHandler
{
    private readonly IReminderMessageParser parser;

    public ReminderMessageHandler(IReminderMessageParser parser)
    {
        this.parser = parser;
    }

    private static async Task SendSuccessMessageToClient(Chat chat, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chat, "Done");
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
}
