
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public static class PhysicsVelocityExtension
{
    public static float3 GetAngularVelocity(this PhysicsVelocity physicsVelocity, PhysicsMass physicsMass, Rotation rotation)
    {
        return physicsMass.ConvertAngularVelocityToWorld(rotation, physicsVelocity.Angular);
    }

    public static void SetAngularVelocity(this ref PhysicsVelocity physicsVelocity, PhysicsMass physicsMass, Rotation rotation, float3 angularVelocity)
    {
        physicsVelocity.Angular = physicsMass.ConvertAngularVelocityToLocal(rotation, angularVelocity);
    }

    public static float3 GetVelocity(this PhysicsVelocity physicsVelocity, PhysicsMass physicsMass, Rotation rotation)
    {
        return physicsMass.ConvertVelocityToWorld(rotation, physicsVelocity.Angular);
    }

    public static void SetVelocity(this ref PhysicsVelocity physicsVelocity, PhysicsMass physicsMass, Rotation rotation, float3 velocity)
    {
        physicsVelocity.Linear = physicsMass.ConvertVelocityToLocal(rotation, velocity);
    }
}