
using Unity.Entities;

public struct ShipEntityData_Behaviour_AvoidWalls : IComponentData
{
    public int bufferIndex;
    public int priority;
    public float maxStrength;
    public float minAccelerationDistance;
    public float maxAccelerationDistance;
}