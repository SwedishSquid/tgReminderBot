using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public class ReminderMessageFormatAttribute: Attribute
{
    public readonly string Pattern;
    public readonly string Description;

    public ReminderMessageFormatAttribute(string reminderMessagePattern, string description)
    {
        Pattern = reminderMessagePattern;
        Description = description;
    }
}
