using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public record Reminder(DateTime TimeToRemind = default, string text = "");
