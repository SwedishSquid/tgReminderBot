using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure;

public interface IKVStorage<TKey, TValue>
    where TKey : notnull
{

    string StorageName { get; }

    public Task UpsertAsync(TKey key, TValue value);

    public Task DeleteAsync(TKey key);

    public bool TryGet(TKey key, out TValue value);

    public IEnumerable<KeyValuePair<TKey, TValue>> Enumerate();

    public Task StartSavingCycle();
}
