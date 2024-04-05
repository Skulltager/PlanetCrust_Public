
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PrepareSystemGroup))]
[UpdateAfter(typeof(ShipSystem_Player_Rotation))]
public partial class ShipSystem_Player_Movement : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_Player_Movement
        {
            desiredDirection = Camera.main.transform.rotation * ControlManager.instance.GetMoveDirection(),
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_PlayerControlled))]
    private partial struct ShipJob_Player_Movement : IJobEntity
    {
        [ReadOnly] public float3 desiredDirection;

        private void Execute(
            ref ShipEntityData_DesiredVelocity desiredVelocity,
            in ShipEntityData_TopSpeed topSpeed)
        {
            desiredVelocity.desiredVelocity = desiredDirection * topSpeed.value;
        }
    }
}