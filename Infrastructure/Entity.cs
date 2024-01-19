using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure;

//thats primary constructor
public class Entity<T>(long id, T payload)
{
    public long Id { get; } = id;

    public T Payload { get; } = payload;
}

public static class Entity
{
    public static Entity<T> Create<T>(long id, T payload)
    {
        return new Entity<T>(id, payload);
    }
}
