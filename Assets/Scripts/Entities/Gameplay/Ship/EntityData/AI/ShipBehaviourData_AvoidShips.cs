using Unity.Entities;

public class ShipBehaviourData_AvoidShips : ShipBehaviourData
{
    public float minAccelerationDistance;
    public float maxAccelerationDistance;

    public override void AddComponentData(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem, int bufferIndex)
    {
        dstManager.AddComponentData(entity, new ShipEntityData_Behaviour_AvoidShips
        {
            bufferIndex = bufferIndex,
            priority = priority,
            minAccelerationDistance = minAccelerationDistance,
            maxAccelerationDistance = maxAccelerationDistance,
        });
    }
}