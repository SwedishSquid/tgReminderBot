using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Domain;

public class DefaultMessageHandler : IMessageHandler
{
    public bool TryHandleMessage(Message message, ITelegramBotClient bot)
    {
        SendMessageToClient(message.Chat, bot).Wait();
        return true;
    }

    private async Task SendMessageToClient(Chat chat, ITelegramBotClient client)
    {
        await client.SendTextMessageAsync(chat, "Incorrect format");
    }
}
