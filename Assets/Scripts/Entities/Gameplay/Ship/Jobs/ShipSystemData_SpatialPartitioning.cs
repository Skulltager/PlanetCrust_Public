
using Unity.Collections;
using Unity.Mathematics;

public struct ShipSystemData_SpatialPartitioning
{
    public NativeParallelMultiHashMap<int3, ShipSystemData_SpatialPartitioning_Ship> spatialPartitioningMap;
    public float largestShipRadius;
}