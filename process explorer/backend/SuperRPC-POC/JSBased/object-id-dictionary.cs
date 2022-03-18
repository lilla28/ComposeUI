﻿using System.Collections.Generic;
namespace SuperRPC;

public class ObjectIdDictionary<TKeyId, TKeyObj, TValue>
    where TKeyId : notnull
    where TKeyObj : notnull
{
    public record Entry(TKeyId id, TKeyObj obj, TValue value);

    public Dictionary<TKeyId, Entry> ById = new Dictionary<TKeyId, Entry>();
    public Dictionary<TKeyObj, Entry> ByObj = new Dictionary<TKeyObj, Entry>();

    public void Add(TKeyId id, TKeyObj obj, TValue value)
    {
        var entry = new Entry(id, obj, value);
        ById.Add(id, entry);
        ByObj.Add(obj, entry);
    }

    public void RemoveById(TKeyId id)
    {
        if (ById.TryGetValue(id, out var entry))
        {
            ByObj.Remove(entry.obj);
            ById.Remove(id);
        }
    }


}