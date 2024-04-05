using Unity.Entities;

public class ShipBehaviourData_KeepDistanceFromPlayer : ShipBehaviourData
{
    public float desiredDistance;

    public override void AddComponentData(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem, int bufferIndex)
    {
        dstManager.AddComponentData(entity, new ShipEntityData_Behaviour_KeepDistanceFromPlayer
        {
            bufferIndex = bufferIndex,
            priority = priority,
            desiredDistance = desiredDistance,
        });
    }
}