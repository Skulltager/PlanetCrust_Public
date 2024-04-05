
using Unity.Entities;
using Unity.Mathematics;

public struct ShipEntityData_DesiredRotation : IComponentData
{
    public quaternion desiredRotation;
}