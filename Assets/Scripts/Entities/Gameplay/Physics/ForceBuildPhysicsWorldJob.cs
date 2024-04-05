
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Physics.Systems;
using Unity.Burst;

[UpdateInGroup(typeof(PreparePhysicsPhaseSystemGroup))]
public partial class ForceBuildPhysicsWorldJob : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        buildPhysicsWorld.Update();
    }
}