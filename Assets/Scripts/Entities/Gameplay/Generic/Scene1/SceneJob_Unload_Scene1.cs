﻿
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class SceneSystem_Unload_Scene1 : SystemBase
{
    private Entity singletonEntity;

    public void UnloadScene()
    {
        EntityManager.AddComponentData(singletonEntity, new SceneTag_Unload_Scene1 { });
    }

    protected override void OnCreate()
    {
        singletonEntity = EntityManager.CreateEntity();
        RequireSingletonForUpdate<SceneTag_Unload_Scene1>();
    }

    protected override void OnUpdate()
    {
        new ShipJob_Unload_Reference()
        {
        }.Run();

        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        JobHandle handle = new SceneJob_Unload_Scene1
        {
            commandBuffer = commandBuffer.AsParallelWriter(),
        }.ScheduleParallel();

        handle.Complete();
        commandBuffer.Playback(EntityManager);
        EntityManager.RemoveComponent<SceneTag_Unload_Scene1>(singletonEntity);
        commandBuffer.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(SceneTag_Scene1))]
    public partial struct SceneJob_Unload_Scene1 : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;

        public void Execute(
            [EntityInQueryIndex] int entityIndex,
            Entity entity)
        {
            commandBuffer.DestroyEntity(entityIndex, entity);
        }
    }
}