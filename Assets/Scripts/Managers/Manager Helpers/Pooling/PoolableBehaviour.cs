
using System;
using UnityEngine;

public abstract class PoolableBehaviour : MonoBehaviour
{
    [NonSerialized] public int poolingIndex;

    private bool inPool;

    public double time { private set; get; }
    public PrefabPoolBase pool;
    public abstract void Reset(PoolableBehaviour basePrefab);

    public void TakeFromPool(double time)
    {
        this.time = time;
        inPool = false;
    }

    public virtual void ReturnToPool()
    {
        if (inPool)
            return;

        inPool = true;
        pool.ReturnToPool(this);
    }
}