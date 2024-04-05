using Unity.Entities;

public class ShipBehaviourData_FollowPath : ShipBehaviourData
{
    public float minimumDistanceToPathNode;
    public float baseSpeedThroughCorners;

    public override void AddComponentData(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem, int bufferIndex)
    {
        dstManager.AddComponentData(entity, new ShipEntityData_Behaviour_FollowPath
        {
            bufferIndex = bufferIndex,
            priority = priority,
            minimumDistanceToPathNode = minimumDistanceToPathNode,
            baseSpeedThroughCorners = baseSpeedThroughCorners,
        });
    }
}