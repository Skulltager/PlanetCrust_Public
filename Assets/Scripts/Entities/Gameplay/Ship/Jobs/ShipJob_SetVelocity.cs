
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[UpdateInGroup(typeof(PreUpdateSystemGroup))]
public partial class ShipSystem_SetVelocity : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_SetVelocity()
        {
            fixedDeltaTime = Time.DeltaTime,
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag))]
    private partial struct ShipJob_SetVelocity : IJobEntity
    {
        public float fixedDeltaTime;

        private void Execute(
            ref PhysicsVelocity physicsVelocity,
            in ShipEntityData_DesiredVelocity desiredVelocity,
            in ShipEntityData_Acceleration acceleration)
        {
            physicsVelocity.Linear = math2.MoveTowards(physicsVelocity.Linear, desiredVelocity.desiredVelocity, acceleration.value * fixedDeltaTime);
        }
    }
}
