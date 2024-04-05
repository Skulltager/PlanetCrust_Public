
using Unity.Collections;
using Unity.Mathematics;

public class PathFindingSaveData_Level
{
    public int width;
    public int height;
    public int depth;
    public float3 offset;
    public PathFindingSaveData_Node[] nodes;

    public PathFindingSaveData_Level(PathFindingData_Level levelData)
    {
        width = levelData.width;
        height = levelData.height;
        depth = levelData.depth;
        offset = levelData.offset;
        nodes = new PathFindingSaveData_Node[levelData.pathFindingNodes.Length];
        for(int i = 0; i < nodes.Length; i++)
            nodes[i] = new PathFindingSaveData_Node(levelData.pathFindingNodes[i]);
    }

    public PathFindingData_Level CreateData()
    {
        NativeArray<PathFindingData_Node> nodeArray = new NativeArray<PathFindingData_Node>(nodes.Length, Allocator.Persistent);
        for (int i = 0; i < nodes.Length; i++)
            nodeArray[i] = nodes[i].CreateData();

        return new PathFindingData_Level()
        {
            width = width,
            height = height,
            depth = depth,
            offset = offset,
            pathFindingNodes = nodeArray,
        };
    }
}