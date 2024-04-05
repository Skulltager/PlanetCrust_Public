
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_ReferenceUpdate_CameraFollow : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        new ShipJob_ReferenceUpdate_CameraFollow()
        {
            commandBuffer = commandBufferSystem.CreateCommandBuffer(),
        }.Run();
    }

    [WithAll(typeof(ShipEntityTag), typeof(ShipEntityTag_CameraFollow))]
    private partial struct ShipJob_ReferenceUpdate_CameraFollow : IJobEntity
    {
        public EntityCommandBuffer commandBuffer;
        private void Execute(
            Entity entity,
            ShipEntityReference shipEntityReference)
        {
            ShipData.controllingShip.value = shipEntityReference.value;
            commandBuffer.RemoveComponent<ShipEntityTag_CameraFollow>(entity);
        }
    }
}