
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(PrepareSystemGroup))]
public partial class ShipSystem_AI_SetDesiredVelocity : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_AI_SetDesiredVelocity
        {
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_SetDesiredVelocity : IJobEntity
    {
        private void Execute(
            ref ShipEntityData_DesiredVelocity desiredVelocity,
            in DynamicBuffer<BufferElement_DesiredVelocity> desiredVelocityBuffer)
        {
            int highestPriority = int.MinValue;

            for (int i = 0; i < desiredVelocityBuffer.Length; i++)
            {
                BufferElement_DesiredVelocity item = desiredVelocityBuffer[i];
                highestPriority = math.max(item.priority, highestPriority);
            }

            float3 totalVelocity = Vector3.zero;
            float totalWeight = 0;
            for (int i = 0; i < desiredVelocityBuffer.Length; i++)
            {
                BufferElement_DesiredVelocity item = desiredVelocityBuffer[i];
                if (item.priority >= 0 && item.priority < highestPriority)
                    continue;

                totalWeight += item.weight;
                totalVelocity += item.desiredVelocity;
            }

            if (totalWeight > 1)
                totalVelocity /= totalWeight;

            desiredVelocity.desiredVelocity = totalVelocity;
        }
    }
}