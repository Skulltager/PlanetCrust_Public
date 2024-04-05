
using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(50)]
public struct BufferElement_PathFindingNode : IBufferElementData
{
    public float3 pathNodePosition;

    public static implicit operator float3(BufferElement_PathFindingNode item)
    {
        return item.pathNodePosition;
    }

    public static implicit operator BufferElement_PathFindingNode(float3 item)
    {
        return new BufferElement_PathFindingNode { pathNodePosition = item };
    }
}