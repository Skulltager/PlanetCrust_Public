
using Unity.Entities;

public struct ShipEntityData_Behaviour_KeepDistanceFromPlayer : IComponentData
{
    public int bufferIndex;
    public int priority;
    public float desiredDistance;
}