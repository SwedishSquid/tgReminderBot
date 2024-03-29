﻿using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain;

[ReminderMessageFormat("(dd.mm.yyyy)? hh:mm <reminder text>", 
    "missing numbers should be filled in with zeros")]
public class ReminderMessageParser : IReminderMessageParser
{
    private static readonly Regex matchRe = new(@"^((?:\d{2}\.\d{2}\.\d{4}\s)?\s*\d{2}:\d{2})\s+(.+)",
                                                RegexOptions.Singleline);

    public bool TryParseReminderMessage(string messageText, ChatData chatData, out Reminder reminder)
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
