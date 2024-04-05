
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Physics;

[UpdateInGroup(typeof(EndSystemGroup), OrderFirst = true)]
[AlwaysSynchronizeSystem]
public partial class ForcePhysicsJobCompletionSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadWrite();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new ForcePhysicsJobCompletionJob
        {

        }.ScheduleParallel();
        handle.Complete();
    }
}

public partial struct ForcePhysicsJobCompletionJob : IJobEntity
{
    public void Execute(
        ref PhysicsVelocity velocity,
        ref PhysicsMass mass)
    {

    }
}