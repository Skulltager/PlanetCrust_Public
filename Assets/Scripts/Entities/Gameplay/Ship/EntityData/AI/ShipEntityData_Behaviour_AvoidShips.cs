
using Unity.Entities;

public struct ShipEntityData_Behaviour_AvoidShips : IComponentData
{
    public int bufferIndex;
    public int priority;
    public float minAccelerationDistance;
    public float maxAccelerationDistance;
}