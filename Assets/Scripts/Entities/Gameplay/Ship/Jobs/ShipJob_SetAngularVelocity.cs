
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PreUpdateSystemGroup))]
public partial class ShipSystem_SetAngularVelocity : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_SetAngularVelocity()
        {
            fixedDeltaTime = Time.DeltaTime,
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag))]
    private partial struct ShipJob_SetAngularVelocity : IJobEntity
    {
        public float fixedDeltaTime;

        private void Execute(
            ref PhysicsVelocity physicsVelocity,
            in PhysicsMass physicsMass,
            in Rotation rotation,
            in ShipEntityData_DesiredRotation desiredRotation,
            in ShipEntityData_RotationAcceleration rotationAcceleration)
        {
            float3 angularVelocity = physicsVelocity.GetAngularVelocity(physicsMass, rotation);
            float3 forward = math.mul(desiredRotation.desiredRotation, math.forward());
            float3 up = math.mul(desiredRotation.desiredRotation, math.up());
            float3 XYVelocity = GetXYAxis(forward, rotation.Value, angularVelocity, rotationAcceleration.value);
            float3 ZVelocity = GetZAxis(forward, up, rotation.Value, angularVelocity, rotationAcceleration.value);
            physicsVelocity.SetAngularVelocity(physicsMass, rotation, XYVelocity + ZVelocity);
        }

        private float3 GetXYAxis(float3 forward, quaternion rotation, float3 angularVelocity, float rotationAcceleration)
        {
            float3 velocity3D = math.mul(math.inverse(rotation), angularVelocity * Mathf.Rad2Deg);
            float2 velocity = new float2(velocity3D.x, velocity3D.y);

            float forcedMoveDistance = GetDistance(math.length(velocity), rotationAcceleration);

            int cicles = (int)forcedMoveDistance / 360;
            float2 velocityDirection;
            if (velocity.x == 0 && velocity.y == 0)
                velocityDirection = new float2(1, 0);
            else
                velocityDirection = math.normalize(velocity);

            float2 startPositionOnCircle = velocityDirection * (forcedMoveDistance - cicles * 360);
            float2 overflow = velocityDirection * cicles * 360;

            float3 shipForward = math.mul(rotation, math.forward());
            float3 shipUp = math.mul(rotation, math.up());

            float3 difference = math.mul(math.inverse(quaternion.LookRotation(shipForward, shipUp)), forward);

            float distance = math2.Angle(shipForward, forward);
            if (math.isnan(distance))
                distance = 0;

            float xFactor = math.sqrt(difference.x * difference.x + difference.z * difference.z);
            float angle = math.atan2(difference.x, -difference.y * xFactor) * Mathf.Rad2Deg;

            float2 endPositionOnCircle = new float2(distance, 0);
            endPositionOnCircle = math2.Rotate(endPositionOnCircle, angle);

            float2 direction = endPositionOnCircle - startPositionOnCircle;

            if (math.length(direction) > 180)
                endPositionOnCircle -= math.normalize(endPositionOnCircle) * 360;

            float2 finalPositionOnCircle = math2.MoveTowards(startPositionOnCircle, endPositionOnCircle, rotationAcceleration * fixedDeltaTime) + overflow;
            float finalSpeed = GetSpeed(math.length(finalPositionOnCircle), rotationAcceleration);
            float3 finalVelocity;
            if (math.lengthsq(finalPositionOnCircle) > 0)
                finalVelocity = new float3(math.normalize(finalPositionOnCircle), 0);
            else
                finalVelocity = new float3();
            finalVelocity.x *= finalSpeed;
            finalVelocity.y *= finalSpeed;
            finalVelocity = math.mul(rotation, finalVelocity * Mathf.Deg2Rad);

            return finalVelocity;
        }

        private float3 GetZAxis(float3 forward, float3 up, quaternion rotation, float3 angularVelocity, float rotationAcceleration)
        {
            float3 velocity3D = math.mul(math.inverse(rotation), angularVelocity * Mathf.Rad2Deg);
            float velocity = velocity3D.z;
            float forcedMoveDistance;
            if (velocity > 0)
                forcedMoveDistance = GetDistance(velocity, rotationAcceleration);
            else
                forcedMoveDistance = -GetDistance(-velocity, rotationAcceleration);

            float3 shipUp = math.mul(rotation, math.up());
            quaternion lookRotation = quaternion.LookRotation(forward, shipUp);
            float3 lookRotationUp = math.mul(lookRotation, math.up());

            float difference = math2.SignedAngle(lookRotationUp, up, forward);
            if (math.isnan(difference))
                difference = 0;

            float desiredMoveDistance = difference - forcedMoveDistance;
            float desiredSpeed;
            if (desiredMoveDistance > 0)
                desiredSpeed = GetSpeed(desiredMoveDistance, rotationAcceleration);
            else
                desiredSpeed = -GetSpeed(-desiredMoveDistance, rotationAcceleration);

            float newVelocity = math2.MoveTowards(velocity, desiredSpeed, rotationAcceleration * fixedDeltaTime);

            float3 finalVelocity = math.mul(rotation, new float3(0, 0, newVelocity * Mathf.Deg2Rad));
            return finalVelocity;
        }

        private float GetDistance(float speed, float acceleration)
        {
            float timeToStop = speed / acceleration;
            float distance = speed * 0.5f * math.max(0, (timeToStop - fixedDeltaTime / 2));
            return distance;
        }

        private float GetSpeed(float distance, float acceleration)
        {
            return math.max(0, math.sqrt(distance * 2 * acceleration) - acceleration * fixedDeltaTime / 2);
        }
    }
}