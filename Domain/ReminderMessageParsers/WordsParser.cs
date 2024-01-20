using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

[ReminderMessageFormat("tomorrow|today <time> <text>", "for convenience")]
public class WordsParser : IReminderMessageParser
{
    public bool TryParseReminderMessage(string messageText, ChatData chatData, out Reminder reminder)
    {
        var parser = new GeneralParserHelper("tomorrow|today");
        

        if (!parser.TryParse(messageText, out var date, out var time, out var text))
        {
            reminder = null;
            return false;
        }

        if (!TimeSpan.TryParse(time, out var timeSpan)){
            reminder = null;
            return false;
        }

        var dateTime = ParseDate(date, chatData) + timeSpan;

        reminder = new Reminder(dateTime, text);
        return true;
    }

    private DateTime ParseDate(string date, ChatData chatData)
    {
        if (date.ToLower() == "tomorrow")
        {
            return chatData
                .ConvertUtcToLocal(DateTime.UtcNow)
                .AddDays(1)
                .Date;
        }
        if (date.ToLower() == "today")
        {
            return chatData
                .ConvertUtcToLocal(DateTime.UtcNow)
                .Date;
        }
        throw new NotImplementedException();
    }
}
