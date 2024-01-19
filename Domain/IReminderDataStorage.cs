using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public interface IReminderDataStorage
{
    public Task<IEnumerable<Entity<ReminderData>>> PopReminderDataRecordsAsync(int maxCount);

    public Task AddReminderDataAsync(ReminderData data);

    public Task AddReminderDataAsync(Record dataRecord);

    public Task SetChatUtcOffsetAsync(long chatId, TimeSpan offset);

    public Task<ChatData> GetChatDataAsync(long chatId);
}
