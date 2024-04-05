
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(5)]

public struct BufferElement_DesiredVelocity : IBufferElementData
{
    public float3 desiredVelocity;
    public float weight;
    public int priority;
}