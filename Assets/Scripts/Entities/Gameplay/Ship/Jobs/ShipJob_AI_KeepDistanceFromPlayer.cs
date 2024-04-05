
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PrepareSystemGroup))]
[UpdateBefore(typeof(ShipSystem_AI_SetDesiredVelocity))]
public partial class ShipSystem_AI_KeepDistanceFromPlayer : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private CollisionFilter collisionFilter;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        collisionFilter = new CollisionFilter
        {
            BelongsTo = (uint)LayerFlags.Ships,
            CollidesWith = (uint)(LayerFlags.Walls),
        };
    }

    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_AI_KeepDistanceFromPlayer
        {
            physicsWorld = buildPhysicsWorld.PhysicsWorld,
            collisionFilter = collisionFilter,
            timeStep = Time.DeltaTime,
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_KeepDistanceFromPlayer : IJobEntity
    {
        [ReadOnly] public float timeStep;
        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public CollisionFilter collisionFilter;

        private void Execute(
            ref DynamicBuffer<BufferElement_DesiredVelocity> desiredVelocityBuffer,
            in PhysicsVelocity physicsVelocity,
            in PhysicsMass physicsMass,
            in Rotation rotation,
            in Translation translation,
            in ShipEntityData_TargetPosition targetPosition,
            in ShipEntityData_TopSpeed topSpeed,
            in ShipEntityData_Acceleration acceleration,
            in ShipEntityData_Behaviour_KeepDistanceFromPlayer keepDistanceFromPlayer)
        {
            RaycastInput rayCastInput = new RaycastInput
            {
                Start = translation.Value,
                End = targetPosition.targetPosition,
                Filter = collisionFilter,
            };

            // If not in direct line of sight of target. Ignore AI
            if (physicsWorld.CastRay(rayCastInput))
            {
                desiredVelocityBuffer[keepDistanceFromPlayer.bufferIndex] = new BufferElement_DesiredVelocity { };
                return;
            }

            float3 velocity = physicsVelocity.GetVelocity(physicsMass, rotation);
            float3 distanceToTarget = targetPosition.targetPosition - translation.Value;
            float3 directionToTarget = math.normalize(distanceToTarget);
            float distanceToMove = math.length(distanceToTarget) - keepDistanceFromPlayer.desiredDistance;

            float velocityOnPath;
            if (math.lengthsq(velocity) == 0)
                velocityOnPath = 0;
            else
                velocityOnPath = math.length(velocity) * math.dot(directionToTarget, math.normalize(velocity));

            float timeToStop = distanceToMove / acceleration.value;
            float guaranteedMoveDistanceOnPath = velocityOnPath * 0.5f * Mathf.Max(0, (timeToStop - timeStep / 2));

            distanceToMove -= guaranteedMoveDistanceOnPath;
            if (distanceToMove < 0)
            {
                distanceToMove = -distanceToMove;
                directionToTarget = -directionToTarget;
            }
            float desiredSpeed = math.clamp(math.sqrt(distanceToMove * 2 * acceleration.value) - acceleration.value * timeStep / 2, 0, topSpeed.value);
            float3 desiredVelocity = directionToTarget * desiredSpeed;
            desiredVelocityBuffer[keepDistanceFromPlayer.bufferIndex] = new BufferElement_DesiredVelocity
            {
                desiredVelocity = desiredVelocity,
                priority = keepDistanceFromPlayer.priority,
                weight = math.clamp(desiredSpeed / topSpeed.value, 0, 1),
            };
        }
    }
}
