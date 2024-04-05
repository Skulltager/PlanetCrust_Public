using SheetCodes;
using Unity.Entities;
using UnityEngine;

public class WeaponAuthoring_Player : WeaponAuthoring
{
    [SerializeField] private PlayerWeaponIdentifier identifier = default;

    private PlayerWeaponRecord record;
    protected override float cooldown => record.Cooldown;
    protected override float angle => record.Angle;
    protected override float spread => record.Spread;
    protected override int projectileCount => record.ProjectileCount;
    protected override float projectileSpeed => record.Projectile.Speed;
    protected override Entity projectilePrefab => record.Projectile.GetEntity();

    public override void ConvertSub(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        record = identifier.GetRecord();
        dstManager.AddComponentData(entity, new EntityTag_PlayerControlled { });
        dstManager.AddComponentData(entity, new WeaponEntityData_Player_EnergyCost { value = record.EnergyCost });
    }
}