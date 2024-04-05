using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(InitializeSystemGroup))]
public partial class ShipSystem_SpatialPartitioningSetup : SystemBase
{
    public const float CHUNK_SIZE = 5;
    public const int HASHMAP_SIZE = 1000;
    public ShipSystemData_SpatialPartitioning spatialPartioningData { private set; get; }
    private NativeParallelMultiHashMap<int3, ShipSystemData_SpatialPartitioning_Ship> spatialPartitioningMap;

    protected override void OnCreate()
    {
        spatialPartitioningMap = new NativeParallelMultiHashMap<int3, ShipSystemData_SpatialPartitioning_Ship>(HASHMAP_SIZE, Allocator.Persistent);

    }

    protected override void OnUpdate()
    {
        spatialPartitioningMap.Clear();
        ShipJob_SpatialPartitioningSetup job = new ShipJob_SpatialPartitioningSetup
        {
            spatialPartitioningMap = spatialPartitioningMap.AsParallelWriter(),
            largestShipRadius = 0,
        };

        JobHandle handle = job.ScheduleParallel();
        handle.Complete();

        spatialPartioningData = new ShipSystemData_SpatialPartitioning
        {
            spatialPartitioningMap = spatialPartitioningMap,
            largestShipRadius = job.largestShipRadius,
        };
    }

    protected override void OnDestroy()
    {
        spatialPartitioningMap.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag))]
    private partial struct ShipJob_SpatialPartitioningSetup : IJobEntity
    {
        public float largestShipRadius;
        public NativeParallelMultiHashMap<int3, ShipSystemData_SpatialPartitioning_Ship>.ParallelWriter spatialPartitioningMap;

        private void Execute(
            Entity entity,
            in ShipEntityData_Radius radius,
            in Translation translation)
        {
            int xIndex = (int)math.floor(translation.Value.x / ShipSystem_SpatialPartitioningSetup.CHUNK_SIZE);
            int yIndex = (int)math.floor(translation.Value.y / ShipSystem_SpatialPartitioningSetup.CHUNK_SIZE);
            int zIndex = (int)math.floor(translation.Value.z / ShipSystem_SpatialPartitioningSetup.CHUNK_SIZE);
            largestShipRadius = math.max(radius.value, largestShipRadius);
            int3 index = new int3(xIndex, yIndex, zIndex);
            spatialPartitioningMap.Add(index, new ShipSystemData_SpatialPartitioning_Ship
            {
                position = translation.Value,
                entity = entity,
                shipSize = radius.value,
            });
        }
    }
}