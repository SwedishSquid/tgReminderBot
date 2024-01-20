using Domain;
using System;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Application;

public interface IMessageHandlerArguments
{
    ITelegramBotClient BotClient { get; }

    Message Message { get; }

    IStorageHandler StorageHandler { get; }
}
