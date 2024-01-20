using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public class ReminderMessageParserList : IReminderMessageParser
{
    private readonly List<IReminderMessageParser> parsers;
    public ReminderMessageParserList(List<IReminderMessageParser> reminderMessageParsers)
    {
        parsers = reminderMessageParsers;
    }

    public bool TryParseReminderMessage(string messageText, ChatData chatData, out Reminder reminder)
    {
        foreach (var parser in parsers)
        {
            if (parser.TryParseReminderMessage(messageText, chatData, out reminder))
                return true;
        }
        reminder = new Reminder();
        return false;
    }
}
