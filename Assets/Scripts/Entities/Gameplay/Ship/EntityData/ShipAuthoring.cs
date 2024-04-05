using SheetCodes;
using Unity.Entities;
using UnityEngine;

public abstract class ShipAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    protected abstract int hull { get; }
    protected abstract int armor { get; }
    protected abstract int shield { get; }
    protected abstract float radius { get; }
    protected abstract float acceleration { get; }
    protected abstract float rotationAcceleration { get; }
    protected abstract float topSpeed { get; }
    protected abstract int impactResistance { get; }
    protected abstract int baseMinImpactDamage { get; }
    protected abstract int baseMaxImpactDamage { get; }
    protected abstract float minVelocityImpactDamage { get; }
    protected abstract float maxVelocityImpactDamage { get; }
    protected abstract float minVelocityImpactSpeed { get; }


    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        ConvertSub(entity, dstManager, conversionSystem);

        dstManager.AddComponentData(entity, new EntityTag_Initialize { });
        dstManager.AddComponentData(entity, new EntityData_Random { });
        dstManager.AddComponentData(entity, new ShipEntityTag { });

        dstManager.AddComponentData(entity, new ShipEntityReference { });
        dstManager.AddComponentData(entity, new ShipEntityReferenceIndex { });

        dstManager.AddComponentData(entity, new ShipEntityData_Acceleration { value = acceleration });
        dstManager.AddComponentData(entity, new ShipEntityData_DesiredRotation { });
        dstManager.AddComponentData(entity, new ShipEntityData_DesiredVelocity { });
        dstManager.AddComponentData(entity, new ShipEntityData_Radius { value = radius });
        dstManager.AddComponentData(entity, new ShipEntityData_RotationAcceleration { value = rotationAcceleration });
        dstManager.AddComponentData(entity, new ShipEntityData_TopSpeed { value = topSpeed });

        dstManager.AddComponentData(entity, new ShipEntityData_TargetPosition { });
        dstManager.AddComponentData(entity, new ShipEntityData_Team { });

        dstManager.AddComponentData(entity, new EntityData_ImpactDamage
        {
            baseMinImpactDamage = baseMinImpactDamage,
            baseMaxImpactDamage = baseMaxImpactDamage,
            maxVelocityImpactDamage = maxVelocityImpactDamage,
            minVelocityImpactDamage = minVelocityImpactDamage,
            minVelocityImpactSpeed = minVelocityImpactSpeed,
        });

        dstManager.AddComponentData(entity, new ShipEntityData_Resistances 
        { 
            impactResistance = impactResistance,
        });

        dstManager.AddComponentData(entity, new ShipEntityData_Health
        {
            hull = hull,
            maxHull = hull,
            armor = armor,
            maxArmor = armor,
            shield = shield,
            maxShield = shield,
        });
    }

    public abstract void ConvertSub(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem);
}