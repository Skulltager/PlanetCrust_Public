using SheetCodes;
using Unity.Entities;
using UnityEngine;

public class ProjectileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private ProjectileIdentifier identifier = default;
    private ProjectileRecord record;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        record = identifier.GetRecord();

        dstManager.AddComponentData(entity, new EntityTag_Initialize { });
        dstManager.AddComponentData(entity, new EntityData_Random { });
        dstManager.AddComponentData(entity, new ProjectileEntityTag { });
        dstManager.AddComponentData(entity, new ProjectileEntityData_Velocity { });
        dstManager.AddComponentData(entity, new ProjectileEntityData_Owner { });

        dstManager.AddComponentData(entity, new ProjectileEntityData_Damage
        {
            damageType = record.DamageType,
            minDamage = record.MinDamage,
            maxDamage = record.MaxDamage,
        });

        dstManager.AddComponentData(entity, new ProjectileEntityData_Life 
        { 
            distanceRemaining = record.LifeDistance,
            timeRemaining = record.LifeTime,
        });
    }
}