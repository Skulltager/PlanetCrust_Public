using SheetCodes;
using Unity.Entities;
using UnityEngine;

public class ShipAuthoring_AI : ShipAuthoring
{
    [SerializeField] private AiShipIdentifier identifier = default;

    private AiShipRecord record;

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
        ShipBehaviourData[] shipBehaviourDatas = GetComponents<ShipBehaviourData>();
        record = identifier.GetRecord();
        
        DynamicBuffer<BufferElement_DesiredVelocity> desiredVelocityBuffer = dstManager.AddBuffer<BufferElement_DesiredVelocity>(entity);

        for (int i = 0; i < shipBehaviourDatas.Length; i++)
            desiredVelocityBuffer.Add(new BufferElement_DesiredVelocity());

        for (int i = 0; i < shipBehaviourDatas.Length; i++)
            shipBehaviourDatas[i].AddComponentData(entity, dstManager, conversionSystem, i);

        dstManager.AddBuffer<BufferElement_PathFindingNode>(entity);

        dstManager.AddComponentData(entity, new EntityTag_AIControlled { });
        dstManager.AddComponentData(entity, new ShipEntityData_AI_PathFindingTimer { });
        dstManager.AddComponentData(entity, new ShipEntityData_AI_Target { });

        dstManager.AddComponentData(entity, new ShipEntityData_AI_PathFindingInterval
        {
            minValue = record.PathFindingMinInterval,
            maxValue = record.PathFindingMaxInterval,
        });
    }
}