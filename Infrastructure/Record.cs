using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;


namespace Infrastructure;

public record Record(Chat Chat, DateTime TimeToRemind, string Message);
