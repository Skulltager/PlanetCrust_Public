using UnityEngine;

public class WeaponData_Transform
{
    public readonly WeaponData weaponData;
    public readonly EventVariable<WeaponData_Transform, Vector3> position;
    public readonly EventVariable<WeaponData_Transform, Quaternion> rotation;

    public WeaponData_Transform(WeaponData weaponData, WeaponTransform weaponTransform)
    {
        this.weaponData = weaponData;
        this.position = new EventVariable<WeaponData_Transform, Vector3>(this, weaponTransform.position);
        this.rotation = new EventVariable<WeaponData_Transform, Quaternion>(this, weaponTransform.rotation);
    }
}