using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Domain;

public interface IMessageHandlerArguments
{
    ITelegramBotClient BotClient { get; }

    Message Message { get; }

    IReminderDataStorage ReminderDataStorage { get; }
}
