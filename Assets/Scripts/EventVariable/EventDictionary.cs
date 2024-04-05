using System.Collections.Generic;
using System.Linq;

public class EventDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    #region Variables
    public delegate void DictionaryEvent(TKey key, TValue value);
    public event DictionaryEvent onAdd;
    public event DictionaryEvent onRemove;
    #endregion

    #region Private Functions
    private void TriggerOnAdd(TKey key, TValue value)
    {
        if (onAdd != null)
            onAdd.Invoke(key, value);
    }
    private void TriggerOnRemove(TKey key, TValue value)
    {
        if (onRemove != null)
            onRemove.Invoke(key, value);
    }
    #endregion

    #region Public Functions
    new public void Add(TKey key, TValue value)
    {
        base.Add(key, value);
        TriggerOnAdd(key, value);
    }

    new public void Remove(TKey key)
    {
        TValue value;
        if (TryGetValue(key, out value) == true)
        {
            base.Remove(key);
            TriggerOnRemove(key, value);
        }
    }

    new public void Clear()
    {
        KeyValuePair<TKey, TValue>[] collection = this.ToArray();
        base.Clear();
        foreach (KeyValuePair<TKey, TValue> item in collection)
            TriggerOnRemove(item.Key, item.Value);
    }

    new public TValue this[TKey index]    // Indexer declaration  
    {
        get { return base[index]; }
        set
        {
            bool newItem = ContainsKey(index);
            base[index] = value;
            if (newItem)
                TriggerOnAdd(index, value);
        }
    }
    #endregion
}