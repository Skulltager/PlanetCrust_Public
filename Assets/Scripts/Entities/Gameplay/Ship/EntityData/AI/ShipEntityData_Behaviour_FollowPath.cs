
using Unity.Entities;

public struct ShipEntityData_Behaviour_FollowPath : IComponentData
{
    public int bufferIndex;
    public int priority;
    public float minimumDistanceToPathNode;
    public float baseSpeedThroughCorners;

    public int pathIndex;
}