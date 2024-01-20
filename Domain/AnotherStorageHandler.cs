using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Infrastructure;

namespace Domain;

public class AnotherStorageHandler : IStorageHandler
{
    private readonly IKVStorage<long, ReminderData> reminders;
    private readonly IKVStorage<long, ChatData> chats;

    private readonly object sheduledQueueLock = new object();
    private readonly PriorityQueue<long, DateTime> sheduledPriority;

    private long nextReminderId = 0;

    public AnotherStorageHandler()
    {
        var relFolderPath = "../../../../Infrastructure/AppData/persistence";
        var remindersName = "reminders";
        var chatsName = "chats";

        reminders = new DictionaryStorage<long, ReminderData>(remindersName, relFolderPath);
        chats = new DictionaryStorage<long, ChatData>(chatsName, relFolderPath);

        sheduledPriority = new PriorityQueue<long, DateTime>();
        FillSheduledPriorityAndRestoreNextReminderId();
    }

    private void FillSheduledPriorityAndRestoreNextReminderId()
    {
        lock (sheduledQueueLock)
        {
            foreach (var kv in reminders.Enumerate())
            {
                nextReminderId = Math.Max(nextReminderId, kv.Key);
                if (kv.Value.State != ReminderState.Sheduled)
                    continue;
                sheduledPriority.Enqueue(kv.Key, kv.Value.NotificationTime);
            }
        }
        nextReminderId++;
    }

    public async Task AddReminderDataAsync(ReminderData reminder)
    {
        var reminderId = GenerateReminderId();
        var chatData = await GetChatDataAsync(reminder.ChatId);
        reminder.NotificationTime = chatData.ConvertLocalToUtc(reminder.NotificationTime);

        await reminders.UpsertAsync(reminderId, reminder);

        lock (sheduledQueueLock)
        {
            sheduledPriority.Enqueue(reminderId, reminder.NotificationTime);
        }
    }

    public async Task AddReminderDataAsync(Record dataRecord)
    {
        await AddReminderDataAsync(new ReminderData(dataRecord.Reminder.TimeToRemind,
            dataRecord.Chat.Id, dataRecord.Reminder.text));
    }

    public async Task<ChatData> GetChatDataAsync(long chatId)
    {
        if (chats.TryGet(chatId, out var chatData))
        {
            return chatData;
        }
        return new ChatData(chatId);
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

            if (!reminders.TryGet(id, out var data))
            {
                Console.WriteLine($"nonexistant reminder record found in sheduled queue: id = {id};");
                //better throw some specific exception
                continue;
            }

            //here other things may be done
            await reminders.DeleteAsync(id);

            records.Add(Entity.Create(id, data));
        }

        return records;
    }

    public async Task SetChatDataAsync(ChatData chatData)
    {
        await chats.UpsertAsync(chatData.Id, chatData);
    }

    public Task StartSavingCycle()
    {
        var remindersTask = reminders.StartSavingCycle();
        var chatsTask = chats.StartSavingCycle();

        //how to combine tasks? (to possibly cancel them when needed)
        //use CancellationToken
        return Task.CompletedTask;
    }

    private long GenerateReminderId()
    {
        return nextReminderId++;
    }
}
