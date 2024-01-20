using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain;

public class SimpleStorageHandler : DictionaryStorageHandler
{
    private readonly string folderPath;
    private readonly string relFolderPath = "../../../../Infrastructure/AppData";
    private readonly string remindersName = "reminders.json";
    private readonly string chatsName = "chats.json";


    public SimpleStorageHandler(string databaseName = "database") : base()
    {
        if (databaseName is null || databaseName == "")
        {
            throw new ArgumentException(nameof(databaseName));
        }

        folderPath = Path.Join(Path.GetFullPath(relFolderPath), databaseName);

        remindersStorage = Load<Dictionary<long, ReminderData>>(remindersName, _remindersStorageLock);

        chatDataStorage = Load<Dictionary<long, ChatData>>(chatsName, _chatDataStorageLock);

        FillSheduledPriority();
    }

    private void FillSheduledPriority()
    {
        lock (sheduledQueueLock)
        {
            lock (_remindersStorageLock)
            {
                foreach (var kv in remindersStorage)
                {
                    nextReminderId = Math.Max(nextReminderId, kv.Key);
                    if (kv.Value.State != ReminderState.Sheduled)
                        continue;
                    sheduledPriority.Enqueue(kv.Key, kv.Value.NotificationTime);
                }
            }
        }
        nextReminderId++;
    }

    private void Dump(object obj, string filename, object objLock)
    {
        var path = Path.Combine(folderPath, filename);
        Directory.CreateDirectory(folderPath);
        lock (objLock)
        {
            var s = JsonSerializer.Serialize(obj);
            using var writer = new StreamWriter(path);
            writer.Write(s);
        }
    }

    private T Load<T>(string filename, object objLock)
        where T : new()
    {
        var path = Path.Join(folderPath, filename);
        if (!File.Exists(path))
        {
            return new T();
        }

        lock (objLock)
        {
            using var reader = new StreamReader(path);
            var s = reader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(s);
        }
    }

    public override Task StartSavingCycle()
    {
        return Task.Run(_StartSavingCycle);
    }

    private void _StartSavingCycle()
    {
        while (true)
        {
            Thread.Sleep(10000);
            Dump(remindersStorage, remindersName, _remindersStorageLock);
            Dump(chatDataStorage, chatsName, _chatDataStorageLock);
        }
    }
}
