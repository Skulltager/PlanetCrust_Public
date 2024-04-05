
using Unity.Entities;
using Unity.Mathematics;

public struct ProjectileEntityData_Life : IComponentData
{
    public float distanceRemaining;
    public float timeRemaining;
}