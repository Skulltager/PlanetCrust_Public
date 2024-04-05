
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(PrepareSystemGroup))]
[UpdateBefore(typeof(ShipSystem_AI_SetDesiredVelocity))]
public partial class ShipSystem_AI_FollowPath : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private CollisionFilter collisionFilter;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();

        collisionFilter = new CollisionFilter()
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
        JobHandle handle = new ShipJob_AI_FollowPath
        {
            physicsWorld = buildPhysicsWorld.PhysicsWorld,
            collisionFilter = collisionFilter,
            currentTime = Time.ElapsedTime,
            deltaTime = Time.DeltaTime,
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_FollowPath : IJobEntity
    {
        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public CollisionFilter collisionFilter;
        [ReadOnly] public double currentTime;
        [ReadOnly] public float deltaTime;

        public void Execute(
            ref DynamicBuffer<BufferElement_DesiredVelocity> desiredVelocityBuffer,
            ref DynamicBuffer<BufferElement_PathFindingNode> path,
            ref ShipEntityData_Behaviour_FollowPath followPath,
            in ShipEntityData_AI_PathFindingTimer pathFindingTimer,
            in ShipEntityData_TargetPosition targetPosition,
            in ShipEntityData_Acceleration acceleration,
            in ShipEntityData_TopSpeed topSpeed,
            in Translation translation)
        {
            if (path.Length == 0)
            {
                desiredVelocityBuffer[followPath.bufferIndex] = new BufferElement_DesiredVelocity { };
                return;
            }

            if (pathFindingTimer.value == currentTime)
            {
                followPath.pathIndex = 1;
            }

            RaycastInput rayCastInput = new RaycastInput
            {
                Start = translation.Value,
                End = targetPosition.targetPosition,
                Filter = collisionFilter,
            };

            // If in direct line of sight of target, ignore AI.
            if (!physicsWorld.CastRay(rayCastInput))
            {
                desiredVelocityBuffer[followPath.bufferIndex] = new BufferElement_DesiredVelocity { };
                return;
            }

            if (followPath.pathIndex >= path.Length)
            {
                desiredVelocityBuffer[followPath.bufferIndex] = new BufferElement_DesiredVelocity { };
                return;
            }

            float3 difference = path[followPath.pathIndex] - translation.Value;
            if (math.length(difference) <= followPath.minimumDistanceToPathNode)
            {
                followPath.pathIndex++;
                if (followPath.pathIndex >= path.Length)
                {
                    desiredVelocityBuffer[followPath.bufferIndex] = new BufferElement_DesiredVelocity { };
                    return;
                }

                difference = path[followPath.pathIndex] - translation.Value;
            }

            float desiredSpeedThroughCorner;
            if (followPath.pathIndex + 1 < path.Length)
            {
                float3 differenceToNext = (float3)path[followPath.pathIndex + 1] - path[followPath.pathIndex];
                float dotDifference = math.dot(math.normalize(difference), math.normalize(differenceToNext));
                desiredSpeedThroughCorner = math.clamp(dotDifference * topSpeed.value + followPath.baseSpeedThroughCorners, 0, topSpeed.value);
            }
            else
            {
                desiredSpeedThroughCorner = 0;
                difference = targetPosition.targetPosition - translation.Value;
            }

            float ignoreDistance = desiredSpeedThroughCorner * desiredSpeedThroughCorner * 0.5f / acceleration.value;

            float calculationDistance = ignoreDistance + math.length(difference);
            float desiredSpeed = math.sqrt(calculationDistance * 2 * acceleration.value) - acceleration.value * deltaTime / 2;
            desiredSpeed = math.min(desiredSpeed, topSpeed.value);
            float3 desiredVelocity = math.normalize(difference) * desiredSpeed;

            desiredVelocityBuffer[followPath.bufferIndex] = new BufferElement_DesiredVelocity
            {
                priority = followPath.priority,
                desiredVelocity = desiredVelocity,
                weight = 1,
            };
        }
    }
}