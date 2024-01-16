using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public interface IReminderMessageParser
{
    public bool TryParseReminderMessage(string messageText, out Reminder reminder);
}
