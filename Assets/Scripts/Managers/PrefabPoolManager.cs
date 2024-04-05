using SheetCodes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PrefabPoolManager : MonoBehaviour
{
    public static PrefabPoolManager instance { private set; get; }

    [SerializeField] private DamageFlyOff damageFlyOffPrefab = default;
    [SerializeField] private int damageFlyOffMaxCount = default;

    private PrefabPool<DamageFlyOff> damageIndicatorPool;

    public void Initialize()
    {
        instance = this;
        damageIndicatorPool = new PrefabPool<DamageFlyOff>(damageFlyOffPrefab, CreateContainer(), damageFlyOffMaxCount);
    }

    public void Reset()
    {
        damageIndicatorPool.Reset();
    }

    private Transform CreateContainer()
    {
        GameObject containerObject = new GameObject();
        containerObject.SetActive(false);
        Transform container = containerObject.transform;
        container.SetParent(transform);
        return container;
    }

    public DamageFlyOff GetDamageFlyOff(RectTransform parent)
    {
        return damageIndicatorPool.GetInstance(parent);
    }
}