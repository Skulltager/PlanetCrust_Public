
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_ReferenceUpdate_Velocity : SystemBase
{
    protected override void OnUpdate()
    {
        new ShipJob_ReferenceUpdate_Velocity()
        {
        }.Run();
    }

    [WithAll(typeof(ShipEntityTag), typeof(ShipEntityTag_TrackVelocity))]
    private partial struct ShipJob_ReferenceUpdate_Velocity : IJobEntity
    {
        private void Execute(
            ShipEntityReference reference,
            in PhysicsVelocity physicsVelocity,
            in PhysicsMass physicsMass,
            in Rotation rotation)
        {
            float3 angularVelocity = physicsVelocity.GetAngularVelocity(physicsMass, rotation);
            reference.value.velocityData.angularVelocity.value = angularVelocity;
            reference.value.velocityData.velocity.value = physicsVelocity.Linear;
        }
    }
}