
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_AI_HighlightNearestObstacle : SystemBase
{
    public NativeList<float3> pointsToHighlight;

    protected override void OnCreate()
    {
        pointsToHighlight = new NativeList<float3>(Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        pointsToHighlight.Clear();
        JobHandle handle = new ShipJob_AI_HighlightNearestObstacle
        {
            pointsToHighlight = pointsToHighlight,
        }.Schedule();
        handle.Complete();

    }

    protected override void OnDestroy()
    {
        pointsToHighlight.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_HighlightNearestObstacle : IJobEntity
    {
        public NativeList<float3> pointsToHighlight;

        private void Execute(
            in DynamicBuffer<BufferElement_PathFindingNode> pathFindingNodes)
        {
            if (pathFindingNodes.Length == 0)
                return;

            for (int i = 0; i < pathFindingNodes.Length; i++)
                pointsToHighlight.Add(pathFindingNodes[i]);
            pointsToHighlight.Add(new float3());
        }
    }
}