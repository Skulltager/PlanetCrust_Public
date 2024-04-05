
using FMOD;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[UpdateInGroup(typeof(PrepareSystemGroup))]
[UpdateBefore(typeof(ShipSystem_AI_SetDesiredVelocity))]
public partial class ShipSystem_AI_AvoidWalls : SystemBase
{
    private PathFindingSystem pathFindingSystem;

    protected override void OnCreate()
    {
        pathFindingSystem = World.GetOrCreateSystem<PathFindingSystem>();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_AI_AvoidWalls
        {
            pathFindingData = pathFindingSystem.pathFindingData,
        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_AvoidWalls : IJobEntity
    {
        [ReadOnly] public PathFindingData_Level pathFindingData;

        private void Execute(
            ref DynamicBuffer<BufferElement_DesiredVelocity> desiredVelocityBuffer,
            in ShipEntityData_TopSpeed topSpeed,
            in ShipEntityData_Radius radius,
            in ShipEntityData_Behaviour_AvoidWalls avoidWalls,
            in Translation translation)
        {
            float3 desiredPosition;
            if (!TryCalculateDesiredPoint(translation.Value, radius.value, avoidWalls.minAccelerationDistance, avoidWalls.maxAccelerationDistance, out desiredPosition))
            {
                desiredVelocityBuffer[avoidWalls.bufferIndex] = new BufferElement_DesiredVelocity() { };
                return;
            }

            float strength = math.length(desiredPosition) / (avoidWalls.minAccelerationDistance - avoidWalls.maxAccelerationDistance);
            strength = math.clamp(strength, 0, 1) * avoidWalls.maxStrength;
            desiredVelocityBuffer[avoidWalls.bufferIndex] = new BufferElement_DesiredVelocity
            {
                desiredVelocity = math.normalize(desiredPosition) * strength * topSpeed.value,
                weight = strength,
                priority = avoidWalls.priority,
            };
        }

        private bool TryCalculateDesiredPoint(float3 worldPosition, float radius, float maxAvoidRadius, float minAvoidRadius, out float3 desiredPoint)
        {
            float3 adjustedWorldPosition = worldPosition - pathFindingData.offset;
            int x = (int)(adjustedWorldPosition.x / PathFindingSystem.DISTANCE_BETWEEN_NODES);
            int y = (int)(adjustedWorldPosition.y / PathFindingSystem.DISTANCE_BETWEEN_NODES);
            int z = (int)(adjustedWorldPosition.z / PathFindingSystem.DISTANCE_BETWEEN_NODES);

            desiredPoint = new float3();

            for (int i = 0; i <= 1; i++)
            {
                for (int j = 0; j <= 1; j++)
                {
                    for (int h = 0; h <= 1; h++)
                    {
                        int3 index3D = new int3(i + x, j + y, h + z);
                        if (IsOutOfBounds(index3D))
                            continue;

                        int index = index3D.x * pathFindingData.height * pathFindingData.depth + index3D.y * pathFindingData.depth + index3D.z;
                        PathFindingData_Node pathNodeData = pathFindingData.pathFindingNodes[index];
                        if (!pathNodeData.wallNearby)
                            continue;

                        float3 obstaclePosition = IndexToWorldPosition(index3D) + pathNodeData.directionToWall;
                        float3 difference = obstaclePosition - worldPosition;
                        float distanceToObstacle = math.length(difference) - radius;
                        if (distanceToObstacle >= maxAvoidRadius)
                            continue;

                        if (distanceToObstacle < minAvoidRadius)
                        {
                            desiredPoint = math.normalize(difference) * (distanceToObstacle - maxAvoidRadius);
                            return true;
                        }

                        desiredPoint += math.normalize(difference) * (distanceToObstacle - maxAvoidRadius);
                    }
                }
            }

            if (math.length(desiredPoint) > maxAvoidRadius)
                desiredPoint = math.normalize(desiredPoint) * maxAvoidRadius;

            return math.lengthsq(desiredPoint) > 0;
        }

        private bool IsOutOfBounds(int3 index)
        {
            if (index.x < 0)
                return true;

            if (index.x >= pathFindingData.width)
                return true;

            if (index.y < 0)
                return true;

            if (index.y >= pathFindingData.height)
                return true;

            if (index.z < 0)
                return true;

            if (index.z >= pathFindingData.depth)
                return true;

            return false;
        }

        private float3 IndexToWorldPosition(int3 index)
        {
            return new float3(index.x * PathFindingSystem.DISTANCE_BETWEEN_NODES, index.y * PathFindingSystem.DISTANCE_BETWEEN_NODES, index.z * PathFindingSystem.DISTANCE_BETWEEN_NODES) + pathFindingData.offset;
        }
    }
}