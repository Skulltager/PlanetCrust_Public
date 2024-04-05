
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializeSystemGroup), OrderLast = true)]
public partial class EntitySystem_Initialize : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new EntityJob_Initialize()
        {
            commandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
        }.ScheduleParallel();

        commandBufferSystem.AddJobHandleForProducer(handle);
    }

    [BurstCompile]
    [WithAll(typeof(EntityTag_Initialize))]
    private partial struct EntityJob_Initialize : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;

        private void Execute(
            Entity entity,
            [EntityInQueryIndex] int entityIndex)
        {
            commandBuffer.RemoveComponent<EntityTag_Initialize>(entityIndex, entity);
        }
    }
}
