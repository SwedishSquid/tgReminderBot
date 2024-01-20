using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Domain;

public class StartMessageHandler : IMessageHandler
{
    public async Task<bool> TryHandleMessageAsync(IMessageHandlerArguments args)
    {
        if (args.Message.Text is not null && args.Message.Text == "/start")
        {
            await SendWelcomMessageAsync(args.Message.Chat, args.BotClient);
            return true;
        }
        return false;
    }

    public static async Task SendWelcomMessageAsync(Chat chat, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chat, "Welcome. use /help to get started");
    }
}
