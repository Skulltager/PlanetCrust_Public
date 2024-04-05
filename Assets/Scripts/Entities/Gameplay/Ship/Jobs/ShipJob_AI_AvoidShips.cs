
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
public partial class ShipSystem_AI_AvoidShips : SystemBase
{
    private ShipSystem_SpatialPartitioningSetup spatialPartitioning;

    protected override void OnCreate()
    {
        spatialPartitioning = World.GetOrCreateSystem<ShipSystem_SpatialPartitioningSetup>();//asd
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_AI_AvoidShips
        {
            spatialPartitioningData = spatialPartitioning.spatialPartioningData,
        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_AvoidShips : IJobEntity
    {
        [ReadOnly] public ShipSystemData_SpatialPartitioning spatialPartitioningData;

        private void Execute(
            Entity entity,
            ref DynamicBuffer<BufferElement_DesiredVelocity> desiredVelocityBuffer,
            in Translation translation,
            in ShipEntityData_Radius radius,
            in ShipEntityData_TopSpeed topSpeed,
            in ShipEntityData_Behaviour_AvoidShips avoidShips)
        {
            float3 desiredPoint;
            if (!TryCalculateDesiredPoint(entity, translation.Value, radius.value, avoidShips.minAccelerationDistance, avoidShips.maxAccelerationDistance, out desiredPoint))
            {
                desiredVelocityBuffer[avoidShips.bufferIndex] = new BufferElement_DesiredVelocity() { };
                return;
            }

            float strength = math.length(desiredPoint) / (avoidShips.minAccelerationDistance - avoidShips.maxAccelerationDistance);
            strength = math.clamp(strength, 0, 1);
            desiredVelocityBuffer[avoidShips.bufferIndex] = new BufferElement_DesiredVelocity
            {
                desiredVelocity = math.normalize(desiredPoint) * strength * topSpeed.value,
                weight = strength,
                priority = avoidShips.priority,
            };
        }

        private bool TryCalculateDesiredPoint(Entity entity, float3 worldPosition, float radius, float maxAvoidRadius, float minAvoidRadius, out float3 desiredPoint)
        {
            float maxRadius = radius + spatialPartitioningData.largestShipRadius + maxAvoidRadius;
            int minX = (int)math.floor((worldPosition.x - maxRadius) / ShipSystem_SpatialPartitioningSetup.CHUNK_SIZE);
            int minY = (int)math.floor((worldPosition.y - maxRadius) / ShipSystem_SpatialPartitioningSetup.CHUNK_SIZE);
            int minZ = (int)math.floor((worldPosition.z - maxRadius) / ShipSystem_SpatialPartitioningSetup.CHUNK_SIZE);
            int maxX = (int)math.ceil((worldPosition.x + maxRadius) / ShipSystem_SpatialPartitioningSetup.CHUNK_SIZE);
            int maxY = (int)math.ceil((worldPosition.y + maxRadius) / ShipSystem_SpatialPartitioningSetup.CHUNK_SIZE);
            int maxZ = (int)math.ceil((worldPosition.z + maxRadius) / ShipSystem_SpatialPartitioningSetup.CHUNK_SIZE);

            desiredPoint = new float3();

            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    for (int h = minZ; h <= maxZ; h++)
                    {
                        int3 index3D = new int3(i, j, h);
                        NativeParallelMultiHashMap<int3, ShipSystemData_SpatialPartitioning_Ship>.Enumerator enumerator = spatialPartitioningData.spatialPartitioningMap.GetValuesForKey(index3D);

                        foreach (ShipSystemData_SpatialPartitioning_Ship shipInRange in enumerator)
                        {
                            if (shipInRange.entity == entity)
                                continue;

                            float3 shipPosition = shipInRange.position;
                            float3 difference = shipPosition - worldPosition;

                            float totalRadius = radius + shipInRange.shipSize;

                            float distanceToShip = math.length(difference) - totalRadius;
                            if (distanceToShip >= maxAvoidRadius)
                                continue;

                            if (distanceToShip <= minAvoidRadius)
                            {
                                desiredPoint = math.normalize(difference) * (distanceToShip - maxAvoidRadius);
                                return true;
                            }

                            desiredPoint += math.normalize(difference) * (distanceToShip - maxAvoidRadius);
                        }
                    }
                }
            }

            if (math.length(desiredPoint) > maxAvoidRadius)
                desiredPoint = math.normalize(desiredPoint) * maxAvoidRadius;

            return math.lengthsq(desiredPoint) > 0;
        }
    }
}