using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain;

public class InMemoryReminderDataStorage : IReminderDataStorage
{
    private readonly object dataStorageLock = new();
    private readonly Dictionary<long, ReminderData> dataStorage;

    private readonly object sheduledQueueLock = new object();
    private readonly PriorityQueue<long, DateTime> sheduledPriority;

    private long nextReminderId = 1;

    public InMemoryReminderDataStorage()
    {
        dataStorage = new Dictionary<long, ReminderData>();
        sheduledPriority = new PriorityQueue<long, DateTime>();
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
                if (!sheduledPriority.TryPeek(out id, out var time) || time > DateTime.Now)
                    break;

                _ = sheduledPriority.Dequeue();
            }

            ReminderData data;
            lock (dataStorageLock)
            {
                data = dataStorage[id];
            }
            records.Add(Entity.Create(id, data));
        }

        return records;
    }

    public async Task AddReminderDataAsync(ReminderData dataPiece)
    {
        var id = GenerateReminderId();
        lock (dataStorageLock)
        {
            dataStorage.Add(id, dataPiece);
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
}
