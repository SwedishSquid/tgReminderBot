using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public interface IStorageHandler
{
    public Task<IEnumerable<Entity<ReminderData>>> PopReminderDataRecordsAsync(int maxCount);

    public Task AddReminderDataAsync(ReminderData data);

    public Task AddReminderDataAsync(Record dataRecord);

    public Task SetChatDataAsync(ChatData chatData);

    public Task<ChatData> GetChatDataAsync(long chatId);

    /// <summary>
    /// saving periodically; nonblocking
    /// </summary>
    public Task StartSavingCycle();
}
