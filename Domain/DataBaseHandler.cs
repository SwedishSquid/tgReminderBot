using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Infrastructure;

namespace Domain;

public static class DataBaseHandler
{
    private static readonly List<Record> records = new List<Record>();

    public static void AddRecord(Chat chat, DateTime timeToRemind, string message)
    {
        records.Add(new Record(chat, timeToRemind, message));
    }

    public static Record? FindClosestRecord()
    {
        if (records.Count == 0)
            return null;
        var currentTime = DateTime.Now;
        return records.MinBy(record => record.TimeToRemind - currentTime);
    }

    public static void RemoveRecord(Record record)
    {
        records.Remove(record);
    }
}
