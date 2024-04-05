
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Physics.Systems;
using Unity.Burst;

[UpdateInGroup(typeof(PostUpdateSystemGroup))]
public partial class TrackerSystem_PostPhysics : SystemBase
{
    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new TrackerJob_PostPhysics()
        {
            localToWorldData = GetComponentDataFromEntity<LocalToWorld>(false),
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    private partial struct TrackerJob_PostPhysics : IJobEntity
    {
        [NativeDisableParallelForRestriction] public ComponentDataFromEntity<LocalToWorld> localToWorldData;
        public void Execute(Entity entity, in TrackerData_PostPhysics trackerData)
        {
            LocalToWorld localToWorld = localToWorldData[entity];
            localToWorld.Value = localToWorldData[trackerData.entity].Value;
            localToWorldData[entity] = localToWorld;
        }
    }
}
