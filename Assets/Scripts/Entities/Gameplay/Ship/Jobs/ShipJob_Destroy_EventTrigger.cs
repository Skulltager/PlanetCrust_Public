
using System;
using System.Reflection;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_Destroy_EventTrigger : SystemBase
{
    private NativeQueue<float3> deathEvents;
    public event Action<float3> onShipDestroyed;

    protected override void OnCreate()
    {
        deathEvents = new NativeQueue<float3>(Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        if (onShipDestroyed == null)
            return;

        JobHandle handle = new ShipJob_Destroy_EventTrigger
        {
            deathEvents = deathEvents.AsParallelWriter(),
        }.ScheduleParallel();
        handle.Complete();

        while (deathEvents.Count > 0)
            onShipDestroyed(deathEvents.Dequeue());
    }

    protected override void OnDestroy()
    {
        deathEvents.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_Destroy))]
    private partial struct ShipJob_Destroy_EventTrigger : IJobEntity
    {
        public NativeQueue<float3>.ParallelWriter deathEvents;

        private void Execute(
            in Translation translation)
        {
            deathEvents.Enqueue(translation.Value);
        }
    }
}
