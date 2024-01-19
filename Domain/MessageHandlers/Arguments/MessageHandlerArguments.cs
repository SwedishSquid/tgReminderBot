using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Domain;

public class MessageHandlerArguments : IMessageHandlerArguments
{
    public ITelegramBotClient BotClient { get; init; }

    public Message Message {  get; set; }

    public IReminderDataStorage ReminderDataStorage { get; init; }

    public MessageHandlerArguments(ITelegramBotClient botClient, IReminderDataStorage reminderDataStorage,
        Message message)
    {
        BotClient = botClient;
        ReminderDataStorage = reminderDataStorage;
        Message = message;
    }
}
