using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;


namespace Domain;

public record Record(Chat Chat, Reminder Reminder);
