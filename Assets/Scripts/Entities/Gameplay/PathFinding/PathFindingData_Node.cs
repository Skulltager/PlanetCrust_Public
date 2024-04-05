
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public unsafe struct PathFindingData_Node
{
    public float3 directionToWall;
    public fixed float maxShipSizes[26];
    public bool wallNearby;
    public bool blocked;
}