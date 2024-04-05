using SheetCodes;
using Unity.Entities;
using UnityEngine;

public class ShipAuthoring_Player : ShipAuthoring
{
    [SerializeField] private PlayerShipIdentifier identifier = default;

    private PlayerShipRecord record;

    protected override int hull => record.Hull;
    protected override int armor => record.Armor;
    protected override int shield => record.Shield;
    protected override float radius => record.Radius;
    protected override float acceleration => record.Acceleration;
    protected override float rotationAcceleration => record.RotationAcceleration;
    protected override float topSpeed => record.TopSpeed;
    protected override int impactResistance => record.ImpactResistance;
    protected override int baseMinImpactDamage => record.BaseMinImpactDamage;
    protected override int baseMaxImpactDamage => record.BaseMaxImpactDamage;
    protected override float minVelocityImpactDamage => record.MinVelocityImpactDamage;
    protected override float maxVelocityImpactDamage => record.MaxVelocityImpactDamage;
    protected override float minVelocityImpactSpeed => record.MinVelocityImpactSpeed;

    public override void ConvertSub(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        record = identifier.GetRecord();

        dstManager.AddComponentData(entity, new EntityTag_PlayerControlled { });

        dstManager.AddComponentData(entity, new ShipEntityTag_TrackTargetPosition { });
        dstManager.AddComponentData(entity, new ShipEntityTag_TrackVelocity { });
        dstManager.AddComponentData(entity, new ShipEntityTag_CameraFollow { });

        dstManager.AddComponentData(entity, new ShipEntityData_Player_AimDistance 
        { 
            minDistance = record.AimMinDistance,
            maxDistance = record.AimMaxDistance,
        });

        dstManager.AddComponentData(entity, new ShipEntityData_Player_Energy
        {
            energy = record.Energy,
            maxEnergy = record.Energy,
            energyRegeneration = record.EnergyRegeneration,
        });
    }
}