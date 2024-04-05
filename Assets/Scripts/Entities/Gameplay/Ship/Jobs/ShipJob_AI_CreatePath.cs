
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
[UpdateBefore(typeof(ShipSystem_AI_FollowPath))]
public partial class ShipSystem_AI_CreatePath : SystemBase
{
    public const int MAX_PATH_FINDING_ATTEMPTS = 200;
    private BuildPhysicsWorld buildPhysicsWorld;
    private PathFindingSystem pathFindingSystem;
    private NativeArray<int3> adjacentOffsets;
    private NativeArray<float> adjacentDistances;
    private CollisionFilter collisionFilter;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        pathFindingSystem = World.GetOrCreateSystem<PathFindingSystem>();

        collisionFilter = new CollisionFilter()
        {
            BelongsTo = (uint)LayerFlags.Ships,
            CollidesWith = (uint)(LayerFlags.Walls),
        };

        adjacentDistances = new NativeArray<float>(26, Allocator.Persistent);
        adjacentOffsets = new NativeArray<int3>(26, Allocator.Persistent);

        int index = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int h = -1; h <= 1; h++)
                {
                    if (i == 0 && j == 0 && h == 0)
                        continue;

                    adjacentDistances[index] = new Vector3(i, j, h).magnitude;
                    adjacentOffsets[index] = new int3(i, j, h);
                    index++;
                }
            }
        }
    }

    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_AI_CreatePath
        {
            adjacentOffsets = adjacentOffsets,
            adjacentDistances = adjacentDistances,
            currentTime = Time.ElapsedTime,
            pathFindingData = pathFindingSystem.pathFindingData,
            collisionFilter = collisionFilter,
            physicsWorld = buildPhysicsWorld.PhysicsWorld,
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    protected override void OnDestroy()
    {
        adjacentDistances.Dispose();
        adjacentOffsets.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_CreatePath : IJobEntity
    {
        [ReadOnly] public PathFindingData_Level pathFindingData;
        [ReadOnly] public double currentTime;
        [ReadOnly] public NativeArray<int3> adjacentOffsets;
        [ReadOnly] public NativeArray<float> adjacentDistances;
        [ReadOnly] public CollisionFilter collisionFilter;
        [ReadOnly] public PhysicsWorld physicsWorld;

        private void Execute(
            ref DynamicBuffer<BufferElement_PathFindingNode> pathFindingNodes,
            ref ShipEntityData_AI_PathFindingTimer pathFindingTimer,
            in ShipEntityData_AI_PathFindingInterval pathFindingInterval,
            in EntityData_Random random,
            in ShipEntityData_Radius radius,
            in ShipEntityData_TargetPosition targetPosition,
            in LocalToWorld localToWorld)
        {
            if (pathFindingTimer.value > currentTime)
                return;

            pathFindingTimer.value = currentTime + random.value.NextDouble(pathFindingInterval.minValue, pathFindingInterval.maxValue);
            NativeList<float3> path = new NativeList<float3>(Allocator.Temp);
            pathFindingNodes.Clear();

            if (!TryCalculatePath(localToWorld.Position, targetPosition.targetPosition, radius.value, path))
            {
                path.Dispose();
                return;
            }

            for (int i = 0; i < path.Length; i++)
                pathFindingNodes.Add(path[i]);

            path.Dispose();
        }

        private unsafe bool TryCalculatePath(float3 startPosition, float3 endPosition, float shipRadius, NativeList<float3> path)
        {
            int3 startLocation;
            if (!TryGetNearestValidPoint(startPosition, shipRadius, out startLocation))
                return false;

            int3 endLocation;
            if (!TryGetNearestValidPoint(endPosition, shipRadius, out endLocation))
                return false;

            if (startLocation.x == endLocation.x && startLocation.y == endLocation.y && startLocation.z == endLocation.z)
            {
                path.Add(IndexToWorldPosition(startLocation));
                path.Add(IndexToWorldPosition(endLocation));
                return true;
            }

            NativeList<EntityPathHandlePoint> pointsToHandle = new NativeList<EntityPathHandlePoint>(Allocator.Temp);
            NativeArray<EntityPathPoint> entityPathPoints = new NativeArray<EntityPathPoint>(pathFindingData.pathFindingNodes.Length, Allocator.Temp);

            int startIndex = ThreeDToOneDIndex(startLocation);
            int endIndex = ThreeDToOneDIndex(endLocation);
            int difference = DistanceBetweenIndex(startLocation, endLocation);
            PathFindingData_Node startNode = pathFindingData.pathFindingNodes[startIndex];
            PathFindingData_Node endNode = pathFindingData.pathFindingNodes[endIndex];

            int pathPointCount = 0;
            entityPathPoints[startIndex] = new EntityPathPoint
            {
                distanceToEnd = difference,
                distanceToStart = 0,
                totalDistance = difference,
            };

            pointsToHandle.Add(new EntityPathHandlePoint
            {
                index = startLocation,
                totalDistance = difference,
            });
            pathPointCount++;

            int countHandled = 0;
            while (pathPointCount > 0)
            {
                countHandled++;

                if (countHandled == ShipSystem_AI_CreatePath.MAX_PATH_FINDING_ATTEMPTS)
                {
                    pointsToHandle.Dispose();
                    entityPathPoints.Dispose();
                    return false;
                }

                EntityPathHandlePoint pointDataToHandle = pointsToHandle[--pathPointCount];
                int pointDataToHandleIndex = ThreeDToOneDIndex(pointDataToHandle.index);
                PathFindingData_Node nodeToHandle = pathFindingData.pathFindingNodes[pointDataToHandleIndex];
                EntityPathPoint pointToHandle = entityPathPoints[pointDataToHandleIndex];
                pointToHandle.listIndex = 0;
                entityPathPoints[pointDataToHandleIndex] = pointToHandle;

                if (pointDataToHandleIndex == endIndex)
                {
                    NativeList<float3> worldPath = new NativeList<float3>(Allocator.Temp);
                    worldPath.Add(endPosition);

                    while (pointToHandle.previousIndex != startIndex)
                    {
                        worldPath.Add(IndexToWorldPosition(OneDIndexToThreeDIndex(pointToHandle.previousIndex)));
                        pointToHandle = entityPathPoints[pointToHandle.previousIndex];
                    }

                    worldPath.Add(IndexToWorldPosition(OneDIndexToThreeDIndex(pointToHandle.previousIndex)));
                    worldPath.Add(startPosition);

                    SmoothPath(worldPath, path, shipRadius);
                    worldPath.Dispose();
                    pointsToHandle.Dispose();
                    entityPathPoints.Dispose();

                    return true;
                }

                for (int i = 0; i < adjacentOffsets.Length; i++)
                {
                    if (nodeToHandle.maxShipSizes[i] < shipRadius)
                        continue;

                    int3 checkLocation = pointDataToHandle.index + adjacentOffsets[i];

                    if (IsOutOfBounds(checkLocation))
                        continue;

                    int checkIndex = ThreeDToOneDIndex(checkLocation);
                    EntityPathPoint pointDataToCheck = entityPathPoints[checkIndex];
                    PathFindingData_Node nodeToCheck = pathFindingData.pathFindingNodes[checkIndex];

                    if (nodeToCheck.blocked && math.length(nodeToCheck.directionToWall) < shipRadius)
                        continue;

                    float distanceToStart = pointToHandle.distanceToStart + adjacentDistances[i];

                    if (pointDataToCheck.totalDistance == 0)
                    {
                        float distanceToEnd = DistanceBetweenIndex(checkLocation, endLocation);
                        pointDataToCheck.distanceToStart = distanceToStart;
                        pointDataToCheck.distanceToEnd = distanceToEnd;
                        pointDataToCheck.totalDistance = distanceToStart + distanceToEnd;
                        pointDataToCheck.previousIndex = ThreeDToOneDIndex(pointDataToHandle.index);
                        entityPathPoints[checkIndex] = pointDataToCheck;

                        EntityPathHandlePoint newHandlePoint = new EntityPathHandlePoint
                        {
                            index = checkLocation,
                            totalDistance = distanceToStart + distanceToEnd,
                        };
                        InsertIntoBuffer(pointsToHandle, entityPathPoints, newHandlePoint, pathPointCount);
                        pathPointCount++;
                        continue;
                    }

                    if (pointDataToCheck.distanceToStart - 0.001 <= distanceToStart)
                        continue;

                    pointDataToCheck.distanceToStart = distanceToStart;
                    pointDataToCheck.totalDistance = distanceToStart + pointDataToCheck.distanceToEnd;
                    entityPathPoints[checkIndex] = pointDataToCheck;

                    EntityPathHandlePoint handlePoint = new EntityPathHandlePoint
                    {
                        index = checkLocation,
                        totalDistance = pointDataToCheck.totalDistance,
                    };

                    if (pointDataToCheck.listIndex == 0)
                    {
                        InsertIntoBuffer(pointsToHandle, entityPathPoints, handlePoint, pathPointCount);
                        pathPointCount++;
                    }
                    else
                    {
                        UpdateBufferPosition(pointsToHandle, entityPathPoints, handlePoint, pointDataToCheck.listIndex, pathPointCount);
                    }
                }
            }

            pointsToHandle.Dispose();
            entityPathPoints.Dispose();
            return false;
        }

        private void SmoothPath(NativeList<float3> roughPath, NativeList<float3> smoothPath, float shipRadius)
        {
            smoothPath.Add(roughPath[roughPath.Length - 1]);
            RaycastInput input = new RaycastInput
            {
                Filter = collisionFilter,
                Start = roughPath[roughPath.Length - 1],
            };

            for (int i = roughPath.Length - 3; i >= 0; i--)
            {
                input.End = roughPath[i];

                if (!physicsWorld.CastRay(input))
                    continue;

                smoothPath.Add(roughPath[i + 1]);
                input.Start = roughPath[i + 1];
            }
            smoothPath.Add(roughPath[0]);
        }

        private int DistanceBetweenIndex(int3 indexA, int3 indexB)
        {
            return math.abs(indexA.x - indexB.x) + math.abs(indexA.y - indexB.y) + math.abs(indexA.z - indexB.z);
        }

        private int ThreeDToOneDIndex(int3 index)
        {
            return index.x * pathFindingData.height * pathFindingData.depth + index.y * pathFindingData.depth + index.z;
        }

        private int3 OneDIndexToThreeDIndex(int index)
        {
            int xIndex = (index / pathFindingData.depth / pathFindingData.height) % pathFindingData.width;
            int yIndex = (index / pathFindingData.depth) % pathFindingData.height;
            int zIndex = index % pathFindingData.depth;
            return new int3(xIndex, yIndex, zIndex);
        }

        private float3 IndexToWorldPosition(int3 index)
        {
            return new float3(index.x * PathFindingSystem.DISTANCE_BETWEEN_NODES, index.y * PathFindingSystem.DISTANCE_BETWEEN_NODES, index.z * PathFindingSystem.DISTANCE_BETWEEN_NODES) + pathFindingData.offset;
        }

        private unsafe bool TryGetNearestValidPoint(float3 position, float shipRadius, out int3 index)
        {
            float3 adjustedPosition = position - pathFindingData.offset;
            float3 scaledPosition = adjustedPosition / PathFindingSystem.DISTANCE_BETWEEN_NODES;

            int xIndex = (int)math.floor(scaledPosition.x);
            int yIndex = (int)math.floor(scaledPosition.y);
            int zIndex = (int)math.floor(scaledPosition.z);

            bool leftFirst = xIndex > scaledPosition.x - 0.5f;
            bool bottomFirst = yIndex > scaledPosition.y - 0.5f;
            bool backFirst = zIndex > scaledPosition.z - 0.5f;

            for (int i = 0; i < 2; i++)
            {
                int xFinalIndex = leftFirst ? xIndex + i : xIndex + 1 - i;
                for (int j = 0; j < 2; j++)
                {
                    int yFinalIndex = bottomFirst ? yIndex + j : yIndex + 1 - j;
                    for (int h = 0; h < 2; h++)
                    {
                        int zFinalIndex = backFirst ? zIndex + h : zIndex + 1 - h;
                        index = new int3(xFinalIndex, yFinalIndex, zFinalIndex);
                        if (IsOutOfBounds(index))
                            continue;

                        int checkIndex = ThreeDToOneDIndex(index);
                        PathFindingData_Node node = pathFindingData.pathFindingNodes[checkIndex];

                        bool found = false;
                        for (int k = 0; k < 26; k++)
                        {
                            float maxShipSize = node.maxShipSizes[k];
                            if (maxShipSize < shipRadius)
                                continue;

                            found = true;
                            break;
                        }

                        if (!found)
                            continue;

                        float3 nodePosition = IndexToWorldPosition(index);
                        RaycastInput rayCastInput = new RaycastInput
                        {
                            Start = position,
                            End = nodePosition,
                            Filter = collisionFilter,
                        };

                        if (physicsWorld.CastRay(rayCastInput))
                            continue;

                        return true;
                    }
                }
            }

            index = default;
            return false;
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

        private void InsertIntoBuffer(NativeList<EntityPathHandlePoint> pointsToHandleBuffer, NativeArray<EntityPathPoint> pathPoints, EntityPathHandlePoint pathHandlePoint, int bufferCount)
        {
            if (bufferCount == pointsToHandleBuffer.Length)
                pointsToHandleBuffer.Add(pathHandlePoint);

            for (int i = bufferCount - 1; i >= 0; i--)
            {
                EntityPathHandlePoint checkHandlePoint = pointsToHandleBuffer[i];
                if (checkHandlePoint.totalDistance <= pathHandlePoint.totalDistance)
                {
                    int checkPointIndex = ThreeDToOneDIndex(checkHandlePoint.index);
                    EntityPathPoint checkPoint = pathPoints[checkPointIndex];
                    checkPoint.listIndex = i + 2;
                    pathPoints[checkPointIndex] = checkPoint;
                    pointsToHandleBuffer[i + 1] = checkHandlePoint;
                    continue;
                }

                int pathPointIndex = ThreeDToOneDIndex(pathHandlePoint.index);
                EntityPathPoint pathPoint = pathPoints[pathPointIndex];
                pathPoint.listIndex = i + 2;
                pathPoints[pathPointIndex] = pathPoint;
                pointsToHandleBuffer[i + 1] = pathHandlePoint;
                return;
            }

            int pathEndPointIndex = ThreeDToOneDIndex(pathHandlePoint.index);
            EntityPathPoint pathEndPoint = pathPoints[pathEndPointIndex];
            pathEndPoint.listIndex = 1;
            pathPoints[pathEndPointIndex] = pathEndPoint;
            pointsToHandleBuffer[0] = pathHandlePoint;
        }

        private void UpdateBufferPosition(NativeList<EntityPathHandlePoint> pointsToHandleBuffer, NativeArray<EntityPathPoint> pathPoints, EntityPathHandlePoint pathHandlePoint, int oldBufferPosition, int bufferCount)
        {
            int currentIndex = oldBufferPosition - 1;

            while (currentIndex + 1 < bufferCount)
            {
                EntityPathHandlePoint checkHandlePoint = pointsToHandleBuffer[currentIndex + 1];
                if (checkHandlePoint.totalDistance < pathHandlePoint.totalDistance)
                    break;

                int checkPointIndex = ThreeDToOneDIndex(checkHandlePoint.index);
                EntityPathPoint checkPoint = pathPoints[checkPointIndex];
                checkPoint.listIndex = currentIndex + 1;
                pathPoints[checkPointIndex] = checkPoint;

                pointsToHandleBuffer[currentIndex] = pointsToHandleBuffer[currentIndex + 1];
                currentIndex++;
            }

            int pathEndPointIndex = ThreeDToOneDIndex(pathHandlePoint.index);
            EntityPathPoint pathEndPoint = pathPoints[pathEndPointIndex];
            pathEndPoint.listIndex = currentIndex + 1;
            pathPoints[pathEndPointIndex] = pathEndPoint;
            pointsToHandleBuffer[currentIndex] = pathHandlePoint;
        }
    }

    private struct EntityPathHandlePoint
    {
        public float totalDistance;
        public int3 index;
    }

    private struct EntityPathPoint
    {
        public float distanceToStart;
        public float distanceToEnd;
        public float totalDistance;
        public int previousIndex;
        public int listIndex;
    }
}

