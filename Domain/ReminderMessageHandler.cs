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
    private static readonly Regex matchRe = new(@"^(\w+)\s+(.+)");
    public bool TryHandleMessage(Message message, ITelegramBotClient bot)
    {
        var text = message.Text.Trim();
        var matchObj = matchRe.Match(text);
        if (!matchObj.Success)
            return false;

        var strDate = matchObj.Groups[1].Value;

        if (DateTime.TryParse(strDate, out var date))
        {
            //if (date < DateTime.Now) 
            //    return false;
            var reminderText = matchObj.Groups[2].Value;
            DataBaseHandler.AddRecord(message.Chat, date, reminderText);
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
