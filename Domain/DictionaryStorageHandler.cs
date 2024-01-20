using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Domain;

public class DictionaryStorageHandler : IStorageHandler
{
    protected readonly object _remindersStorageLock = new();
    protected Dictionary<long, ReminderData> remindersStorage;

    protected readonly object sheduledQueueLock = new object();
    protected readonly PriorityQueue<long, DateTime> sheduledPriority;

    protected readonly object _chatDataStorageLock = new object();
    protected Dictionary<long, ChatData> chatDataStorage;

    protected long nextReminderId = 1;

    public DictionaryStorageHandler()
    {
        remindersStorage = new Dictionary<long, ReminderData>();
        sheduledPriority = new PriorityQueue<long, DateTime>();
        chatDataStorage = new Dictionary<long, ChatData>();
    }

    public async Task<IEnumerable<Entity<ReminderData>>> PopReminderDataRecordsAsync(int maxCount)
    {
        if (maxCount <= 0)
        {
            throw new ArgumentOutOfRangeException($"{nameof(maxCount)} = {maxCount}");
        }
        var records = new List<Entity<ReminderData>>();
        for (int i = 0; i < maxCount; i++)
        {
            long id;
            lock (sheduledQueueLock)
            {
                if (!sheduledPriority.TryPeek(out id, out var time) || time > DateTime.UtcNow)
                    break;

                _ = sheduledPriority.Dequeue();
            }

            ReminderData data;
            lock (_remindersStorageLock)
            {
                data = remindersStorage[id];
                data.State = ReminderState.Sent;
            }
            records.Add(Entity.Create(id, data));
        }

        return records;
    }

    public async Task AddReminderDataAsync(ReminderData dataPiece)
    {
        var id = GenerateReminderId();
        var chatData = await GetChatDataAsync(dataPiece.ChatId);
        dataPiece.NotificationTime = chatData.ConvertLocalToUtc(dataPiece.NotificationTime);

        lock (_remindersStorageLock)
        {
            remindersStorage.Add(id, dataPiece);
        }

        lock (sheduledQueueLock)
        {
            sheduledPriority.Enqueue(id, dataPiece.NotificationTime);
        }
    }

    public async Task AddReminderDataAsync(Record dataRecord)
    {
        await AddReminderDataAsync(new ReminderData(dataRecord.Reminder.TimeToRemind, dataRecord.Chat.Id, dataRecord.Reminder.text));
    }

    private long GenerateReminderId()
    {
        return nextReminderId++;
    }

    public async Task SetChatUtcOffsetAsync(long chatId, TimeSpan offset)
    {
        lock (_chatDataStorageLock)
        {
            if (chatDataStorage.TryGetValue(chatId, out var chatData))
            {
                chatData.UtcOffset = offset;
                chatDataStorage[chatId] = chatData;
                return;
            }
            chatData = new ChatData(chatId, offset);
            chatDataStorage.Add(chatId, chatData);
        }
    }

    public async Task SetChatDataAsync(ChatData chatData)
    {
        var chatId = chatData.Id;
        lock (_chatDataStorageLock)
        {
            if (chatDataStorage.ContainsKey(chatId))
            {
                chatDataStorage[chatId] = chatData;
                return;
            }
            chatDataStorage.Add(chatId, chatData);
        }
    }

    public async Task<ChatData> GetChatDataAsync(long chatId)
    {
        lock (_chatDataStorageLock)
        {
            if (chatDataStorage.TryGetValue(chatId, out var value))
                return value;
            return new ChatData(chatId);
        }
    }

    public virtual Task StartSavingCycle()
    {
        //do nothing
        return Task.CompletedTask;
    }
}
