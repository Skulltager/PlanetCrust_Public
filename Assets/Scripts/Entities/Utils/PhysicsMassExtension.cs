
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public static class PhysicsMassExtension
{
    public static float3 ConvertVelocityToWorld(this PhysicsMass physicsMass, Rotation rotation, float3 velocity)
    {
        quaternion inertiaOrientationInWorldSpace = math.mul(rotation.Value, physicsMass.InertiaOrientation);
        return math.rotate(inertiaOrientationInWorldSpace, velocity);
    }

    public static float3 ConvertVelocityToLocal(this PhysicsMass physicsMass, Rotation rotation, float3 velocity)
    {
        quaternion inertiaOrientationInWorldSpace = math.mul(rotation.Value, physicsMass.InertiaOrientation);
        return math.rotate(math.inverse(inertiaOrientationInWorldSpace), velocity);
    }

    public static float3 ConvertAngularVelocityToWorld(this PhysicsMass physicsMass, Rotation rotation, float3 angularVelocity)
    {
        quaternion inertiaOrientationInWorldSpace = math.mul(rotation.Value, physicsMass.InertiaOrientation);
        return math.rotate(inertiaOrientationInWorldSpace, angularVelocity);
    }

    public static float3 ConvertAngularVelocityToLocal(this PhysicsMass physicsMass, Rotation rotation, float3 angularVelocity)
    {
        quaternion inertiaOrientationInWorldSpace = math.mul(rotation.Value, physicsMass.InertiaOrientation);
        return math.rotate(math.inverse(inertiaOrientationInWorldSpace), angularVelocity);
    }
}