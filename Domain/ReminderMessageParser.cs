using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain;

public class ReminderMessageParser : IReminderMessageParser
{
    private static readonly Regex matchRe = new(@"^((?:\d{2}\.\d{2}\.\d{4}\s)?\s*\d{2}:\d{2})\s+(.+)");

    public bool TryParseReminderMessage(string messageText, out Reminder reminder)
    {
        var matchObj = matchRe.Match(messageText);
        if (matchObj.Success && DateTime.TryParse(matchObj.Groups[1].Value, out var date))
        {
            reminder = new Reminder(date, matchObj.Groups[2].Value);
            return true;
        }
        reminder = new Reminder();
        return false;
    }
}
