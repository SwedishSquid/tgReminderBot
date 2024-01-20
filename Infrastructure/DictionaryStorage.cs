using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure;

public class DictionaryStorage<TKey, TValue> : IKVStorage<TKey, TValue>
    where TKey : notnull
{
    public string StorageName { get; init; }

    private readonly Dictionary<TKey, TValue> dictionary;
    private readonly object _lock = new();

    private readonly string storeFolderPath;
    private readonly int savingTimeout_ms;
    private bool saving = false;

    public DictionaryStorage(string storageName, string storeFolderPath, int savingTimeout_ms = 10000)
    {
        StorageName = storageName;

        this.storeFolderPath = storeFolderPath;
        this.savingTimeout_ms = savingTimeout_ms;

        if (!TryLoad(out dictionary) || dictionary is null)
            dictionary = new();
    }

    public Task DeleteAsync(TKey key)
    {
        lock (_lock)
        {
            dictionary.Remove(key);
        }
        return Task.CompletedTask;
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate()
    {
        //not efficient
        lock (_lock)
        {
            return dictionary.ToImmutableList();
        }
    }

    public Task StartSavingCycle()
    {
        if (saving)
        {
            throw new InvalidOperationException("saving cycle is already running");
        }
        saving = true;

        return Task.Run(RunSavingCycle);
    }

    private void Dump()
    {
        var filepath = Path.Combine(storeFolderPath, $"{StorageName}.json");
        filepath = Path.GetFullPath(filepath);

        if (!Directory.Exists(storeFolderPath))
        {
            Directory.CreateDirectory(storeFolderPath);
        }

        lock (_lock)
        {
            var s = JsonSerializer.Serialize(dictionary);
            using var writer = new StreamWriter(filepath);
            writer.Write(s);
        }
    }

    private bool TryLoad(out Dictionary<TKey, TValue>? result)
    {
        var filepath = Path.Combine(storeFolderPath, $"{StorageName}.json");
        filepath = Path.GetFullPath(filepath);

        if (!File.Exists(filepath))
        {
            result = null;
            return false;
        }

        lock (_lock)
        {
            using var reader = new StreamReader(filepath);
            var s = reader.ReadToEnd();
            result = JsonSerializer.Deserialize<Dictionary<TKey, TValue>>(s);
        }

        return true;
    }

    private void RunSavingCycle()
    {
        while (true)
        {
            Thread.Sleep(savingTimeout_ms);
            Dump();
        }
    }

    public Task UpsertAsync(TKey key, TValue value)
    {
        //not so async though
        lock(_lock)
        {
            if (!dictionary.TryAdd(key, value))
            {
                dictionary[key] = value;
            }
        }
        return Task.CompletedTask;
    }

    public bool TryGet(TKey key, out TValue value)
    {
        lock( _lock)
        {
            if (dictionary.TryGetValue(key, out value))
            {
                return true;
            }
            return false;
        }
    }
}
