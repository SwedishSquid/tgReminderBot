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

public class ReminderMessageHandler: IMessageHandler
{
    private readonly IReminderMessageParser parser;

    public ReminderMessageHandler(IReminderMessageParser parser)
    {
        this.parser = parser;
    }

    //public bool TryHandleMessage(Message message, ITelegramBotClient bot)
    //{
    //    if (parser.TryParseReminderMessage(message.Text, out var reminder))
    //    {
    //        DataBaseHandler.AddRecord(message.Chat, reminder);
    //        SendSuccessMessageToClient(message.Chat, bot).Wait();
    //        return true;
    //    }

    //    return false;
    //}

    private static async Task SendSuccessMessageToClient(Chat chat, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chat, "Done");
    }

    public async Task<bool> TryHandleMessageAsync(IMessageHandlerArguments args)
    {
        if (args.Message.Text is null 
            || !parser.TryParseReminderMessage(args.Message.Text, out var reminder))
        {
            return false;
        }

        await args.StorageHandler.AddReminderDataAsync(
            new ReminderData(reminder.TimeToRemind, args.Message.Chat.Id, reminder.text));

        await SendSuccessMessageToClient(args.Message.Chat, args.BotClient);

        return true;
    }
}
