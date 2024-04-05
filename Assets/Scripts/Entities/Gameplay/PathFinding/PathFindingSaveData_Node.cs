
using System;
using Unity.Mathematics;

public unsafe class PathFindingSaveData_Node
{
    public float3 directionToWall;
    public float[] maxShipSizes;
    public bool wallNearby;
    public bool blocked;

    public PathFindingSaveData_Node(PathFindingData_Node node)
    {
        directionToWall = node.directionToWall;
        maxShipSizes = new float[26];
        for(int i = 0; i < maxShipSizes.Length; i++)
            maxShipSizes[i] = node.maxShipSizes[i];

        wallNearby = node.wallNearby;
        blocked = node.blocked;
    }

    public PathFindingData_Node CreateData()
    {
        PathFindingData_Node data = new PathFindingData_Node
        {
            directionToWall = directionToWall,
            wallNearby = wallNearby,
            blocked = blocked,
        };

        for (int i = 0; i < maxShipSizes.Length; i++)
            data.maxShipSizes[i] = maxShipSizes[i];

        return data;
    }
}