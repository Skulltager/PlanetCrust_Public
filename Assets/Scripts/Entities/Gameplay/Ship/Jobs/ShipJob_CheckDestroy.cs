
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(CheckSystemGroup))]
public partial class ShipSystem_CheckDestroy : SystemBase
{
    private DestroyCommandBufferSystem commandBufferSystem;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetOrCreateSystem<DestroyCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_CheckDestroy
        {
            commandBuffer = commandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
        }.ScheduleParallel();
        commandBufferSystem.AddJobHandleForProducer(handle);
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag))]
    private partial struct ShipJob_CheckDestroy : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;

        private void Execute(
            Entity entity,
            [EntityInQueryIndex] int entityIndex,
            in ShipEntityData_Health health,
            in ShipEntityData_RootEntity rootEntity)
        {
            if (health.hull > 0)
                return;

            commandBuffer.AddComponent<EntityTag_Destroy>(entityIndex, entity);
            commandBuffer.AddComponent<EntityTag_Destroy>(entityIndex, rootEntity.value);
        }
    }
}