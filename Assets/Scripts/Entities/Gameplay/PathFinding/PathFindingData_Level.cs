
using Unity.Collections;
using Unity.Mathematics;

public struct PathFindingData_Level
{
    public int width;
    public int height;
    public int depth;
    public float3 offset;
    public NativeArray<PathFindingData_Node> pathFindingNodes;
}