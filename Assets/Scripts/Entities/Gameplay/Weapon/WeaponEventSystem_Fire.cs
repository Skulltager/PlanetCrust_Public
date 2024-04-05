
using System;
using UnityEngine;

public static class WeaponEventSystem_Fire
{
    public static event Action<WeaponEventData_Fire> onWeaponFire;

    public static void Trigger(WeaponEventData_Fire eventData)
    {
        if (onWeaponFire != null)
            onWeaponFire(eventData);
    }
}

public struct WeaponEventData_Fire
{
    public Vector3 position;
    public Quaternion rotation;
}