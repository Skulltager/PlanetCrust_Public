using System;
using System.Collections.Generic;

public class EventList<T> : List<T>
{
    #region Variables
    public delegate void ListEvent(T item);
    public event ListEvent onAdd;
    public event ListEvent onRemove;
    #endregion

    #region Private Functions
    private void TriggerOnAdd(T item)
    {
        if (onAdd != null)
            onAdd.Invoke(item);
    }

    private void TriggerOnRemove(T item)
    {
        if (onRemove != null)
            onRemove.Invoke(item);
    }
    #endregion

    #region Public Functions
    new public void Add(T item)
    {
        base.Add(item);
        TriggerOnAdd(item);
    }

    new public bool Remove(T item)
    {
        bool result = base.Remove(item);
        if(result)
            TriggerOnRemove(item);
        return result;
    }

    new public void RemoveAt(int index)
    {
        T item = this[index];
        base.RemoveAt(index);
        TriggerOnRemove(item);
    }

    new public int RemoveAll(Predicate<T> match)
    {
        List<int> removedIndexes = new List<int>(Count);
        for (int i = 0; i < Count; i++)
        {
            if (match(this[i]))
                removedIndexes.Add(i);
        }

        if (removedIndexes.Count == 0)
            return 0;

        T[] removedItems = new T[removedIndexes.Count];

        int removedListIndex = 0;
        int currentRemovedIndex = removedIndexes[removedListIndex];
        for (int i = currentRemovedIndex; i < Count; i++)
        {
            if (currentRemovedIndex == i)
            {
                removedItems[removedListIndex++] = this[i];
                if (removedListIndex < removedIndexes.Count)
                    currentRemovedIndex = removedIndexes[removedListIndex];
                else
                    currentRemovedIndex = -1;
            }
            else
                this[i - removedListIndex] = this[i];
        }

        base.RemoveRange(Count - removedListIndex, removedListIndex);

        foreach (T item in removedItems)
            TriggerOnRemove(item);

        return removedItems.Length;
    }

    new public void RemoveRange(int index, int count)
    {
        T[] removedRange = new T[count];
        for (int i = 0; i < count; i++)
            removedRange[i] = this[i + index];

        base.RemoveRange(index, count);

        foreach (T item in removedRange)
            TriggerOnRemove(item);
    }

    new public void AddRange(IEnumerable<T> collection)
    {
        base.AddRange(collection);
        foreach (T item in collection)
            TriggerOnAdd(item);
    }

    new public void Clear()
    {
        T[] collection = ToArray();
        base.Clear();
        foreach(T item in collection)
            TriggerOnRemove(item);
    }

    new public void Insert(int index, T item)
    {
        base.Insert(index, item);
        TriggerOnAdd(item);
    }

    new public void InsertRange(int index, IEnumerable<T> collection)
    {
        base.InsertRange(index, collection);
        foreach (T item in collection)
            TriggerOnAdd(item);
    }
    #endregion
}