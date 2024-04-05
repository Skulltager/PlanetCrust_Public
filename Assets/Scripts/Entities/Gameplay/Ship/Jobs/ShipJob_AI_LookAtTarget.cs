
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PrepareSystemGroup))]
[UpdateAfter(typeof(ShipSystem_AI_SetTargetPosition))]

public partial class ShipSystem_AI_LookAtPlayer : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_AI_LookAtTarget
        {
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_LookAtTarget : IJobEntity
    {
        private void Execute(
            ref ShipEntityData_DesiredRotation desiredRotation,
            in ShipEntityData_TargetPosition targetPosition,
            in Translation translation,
            in Rotation rotation)
        {
            float3 difference = targetPosition.targetPosition - translation.Value;
            float3 forward;
            if (math.lengthsq(difference) > 0.01f)
                forward = math.normalize(difference);
            else
                forward = math.mul(rotation.Value, math.forward());

            desiredRotation.desiredRotation = quaternion.LookRotation(forward, math.mul(rotation.Value, math.up()));
        }
    }
}