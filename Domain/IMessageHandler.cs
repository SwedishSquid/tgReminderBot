using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace Domain;

public interface IMessageHandler
{
    public bool TryHandleMessage(Message message, ITelegramBotClient bot);
}
