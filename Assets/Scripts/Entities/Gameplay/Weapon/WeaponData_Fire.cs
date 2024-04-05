using UnityEngine;

public class WeaponData_Fire
{
    public readonly EventVariable<WeaponData_Fire, float> firingAngle;
    public readonly EventVariable<WeaponData_Fire, float> spread;

    public WeaponData_Fire()
    {
        firingAngle = new EventVariable<WeaponData_Fire, float>(this, default);
        spread = new EventVariable<WeaponData_Fire, float>(this, default);
    }
}