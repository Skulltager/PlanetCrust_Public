
using System.Collections.Generic;
using UnityEngine;

public class PrefabPool<T> : PrefabPoolBase where T : PoolableBehaviour
{
    private readonly List<T> instances;
    private readonly Transform container;
    private readonly int maxInstances;
    public readonly T prefab;

    private int countInUse;

    public PrefabPool(T prefab, Transform container, int maxInstances)
    {
        this.prefab = prefab;
        this.container = container;
        this.maxInstances = maxInstances;

        instances = new List<T>();
        countInUse = 0;
    }

    public void Reset()
    {
        for(int i = 0; i < countInUse; i++)
        {
            T instance = instances[i];
            GameObject.Destroy(instance.gameObject);
        }
        instances.Clear();
        countInUse = 0;
    }

    public T GetInstance(Vector3 worldPosition, Quaternion rotation)
    {
        T instance = GetInstance();
        instance.transform.SetParent(null);
        instance.transform.position = worldPosition;
        instance.transform.rotation = rotation;
        return instance;
    }

    public T GetInstance(Transform parent)
    {
        T instance = GetInstance();
        instance.transform.SetParent(parent);
        return instance;
    }

    private T GetInstance()
    {
        if(countInUse == maxInstances)
        {
            T lowestTimeItem = default;
            double lowestTime = double.MaxValue;
            for(int i = 0; i < countInUse; i++)
            {
                T item = instances[i];
                if (lowestTime < item.time)
                    continue;

                lowestTime = item.time;
                lowestTimeItem = item;
            }

            lowestTimeItem.ReturnToPool();
            countInUse--;
        }
        else if (countInUse == instances.Count)
        { 
            T newItem = GameObject.Instantiate(prefab, container);
            newItem.pool = this;
            instances.Add(newItem);
        }

        T instance = instances[countInUse];
        instance.poolingIndex = countInUse;
        countInUse++;
        instance.TakeFromPool(Time.timeAsDouble);
        instance.Reset(prefab);
        return instance;
    }

    public override void ReturnToPool(PoolableBehaviour baseInstance)
    {
        T instance = baseInstance as T;
        instance.transform.SetParent(container);
        countInUse--;

        if (instance.poolingIndex == countInUse)
            return;

        T temp = instances[countInUse];
        instances[countInUse] = instance;
        temp.poolingIndex = instance.poolingIndex;
        instances[temp.poolingIndex] = temp;
    }
}
