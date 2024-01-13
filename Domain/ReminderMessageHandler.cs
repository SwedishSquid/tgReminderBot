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

public class ReminderMessageHandler : IMessageHandler
{
    private static readonly Regex match_re = new(@"^(\w+) (\.+)");
    public bool TryHandleMessage(Message message, ITelegramBotClient bot)
    {
        var text = message.Text.Trim();
        var matchObj = match_re.Match(text);
        if (!matchObj.Success)
            return false;

        if (DateTime.TryParse(matchObj.Groups[0].Value, out var date))
        {
            //if (date < DateTime.Now) 
            //    return false;

            DataBaseHandler.AddRecord(message.Chat, date, text);
            SendSuccessMessageToClient(message.Chat, bot).Wait();
            return true;
        }

        return false;
    }

    private async Task SendSuccessMessageToClient(Chat chat, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(chat, "Done");
    }
}
