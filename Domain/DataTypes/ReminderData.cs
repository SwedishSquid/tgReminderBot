using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public class ReminderData 
{
    public DateTime NotificationTime { get; set; }

    public long ChatId { get; set; }

    public string TextContent { get; set; }

    public ReminderState State { get; set; }

    public ReminderData(DateTime notificationTime, long chatId, string textContent)
    {
        NotificationTime = notificationTime;
        ChatId = chatId;
        TextContent = textContent;
        State = ReminderState.Sheduled;
    }
}
