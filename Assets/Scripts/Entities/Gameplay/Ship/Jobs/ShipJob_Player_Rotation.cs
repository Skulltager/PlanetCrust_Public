
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PrepareSystemGroup))]
public partial class ShipSystem_Player_Rotation : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_Player_Rotation()
        {
            cameraRotation = Camera.main.transform.rotation,
        }.ScheduleParallel();

        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_PlayerControlled))]
    private partial struct ShipJob_Player_Rotation : IJobEntity
    {
        public quaternion cameraRotation;

        private void Execute(
            ref ShipEntityData_DesiredRotation desiredRotation)
        {
            desiredRotation.desiredRotation = cameraRotation;
        }
    }
}
