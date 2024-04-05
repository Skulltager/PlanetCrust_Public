
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(EndSystemGroup), OrderLast = true)]
public partial class EntitySystem_Destroy : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem commandBufferSystem;
    private EntityQuery entityQuery;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new EntityJob_Destroy()
        {
            commandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
        }.ScheduleParallel();

        commandBufferSystem.AddJobHandleForProducer(handle);
    }
}

[BurstCompile]
[WithAll(typeof(EntityTag_Destroy))]
public partial struct EntityJob_Destroy : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter commandBuffer;

    private void Execute(
        Entity entity,
        [EntityInQueryIndex] int entityIndex)
    {
        commandBuffer.DestroyEntity(entityIndex, entity);
    }
}
