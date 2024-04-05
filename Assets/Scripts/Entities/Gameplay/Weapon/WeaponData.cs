
using Unity.Entities;

public class WeaponData
{
    public readonly ShipData shipData;
    public readonly Entity entity;
    public readonly WeaponData_Transform[] transformDatas;
    public readonly WeaponData_Fire fireData;

    public WeaponData(ShipData shipData, Entity entity, WeaponTransform[] weaponTransforms)
    {
        this.shipData = shipData;
        this.entity = entity;
        transformDatas = new WeaponData_Transform[weaponTransforms.Length];
        for (int i = 0; i < transformDatas.Length; i++)
            transformDatas[i] = new WeaponData_Transform(this, weaponTransforms[i]);
        fireData = new WeaponData_Fire();
        shipData.weaponDatas.Add(this);
    }
}