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
    public Task<bool> TryHandleMessageAsync(IMessageHandlerArguments args);

    public static virtual string? GetDetailedHelp() => null;
}
