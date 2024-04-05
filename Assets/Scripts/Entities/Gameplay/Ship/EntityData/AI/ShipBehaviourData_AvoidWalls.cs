using Unity.Entities;

public class ShipBehaviourData_AvoidWalls : ShipBehaviourData
{
    public float maxStrength;
    public float minAccelerationDistance;
    public float maxAccelerationDistance;

    public override void AddComponentData(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem, int bufferIndex)
    {
        dstManager.AddComponentData(entity, new ShipEntityData_Behaviour_AvoidWalls
        {
            bufferIndex = bufferIndex,
            priority = priority,
            maxStrength = maxStrength,
            minAccelerationDistance = minAccelerationDistance,
            maxAccelerationDistance = maxAccelerationDistance,
        });
    }
}