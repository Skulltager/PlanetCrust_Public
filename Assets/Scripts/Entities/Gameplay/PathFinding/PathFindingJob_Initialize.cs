
using SheetCodes;
using System.IO;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(InitializeSystemGroup))]
public partial class PathFindingSystem : SystemBase
{
    public const float DISTANCE_BETWEEN_NODES = 5;
    public const float SHIP_SIZE_SAMPLE_START = 4;
    public const float SHIP_SIZE_SAMPLE_REDUCTION = 0.2f;
    public const float WALL_CHECK_DISTANCE = 20;

    private static string SAVE_DIRECTORY = string.Format("{0}/{1}", "Resources", RESOURCE_DIRECTORY);
    private const string RESOURCE_DIRECTORY = "Path Finding Data";
    private static string LOCAL_DIRECTORY => string.Format("{0}/{1}", SceneLoader.applicationPath, SAVE_DIRECTORY);
    private const string SAVE_FILE_EXTENTION = "bytes";
    public PathFindingData_Level pathFindingData { private set; get; }

    private EndFixedStepSimulationEntityCommandBufferSystem commandBufferSystem;
    private BuildPhysicsWorld buildPhysicsWorld;
    private Entity singletonEntity;
    private CollisionFilter collisionFilter;

    private NativeArray<PathFindingData_Node> pathFindingNodes;
    private int width;
    private int height;
    private int depth;
    private float3 offset;
    private bool initialized;
    private bool createMapData;

    protected override void OnCreate()
    {
        singletonEntity = EntityManager.CreateEntity();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        commandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<PathFindingTag_Initialize>();
        collisionFilter = new CollisionFilter()
        {
            BelongsTo = (uint)LayerFlags.Projectiles,
            CollidesWith = (uint)(LayerFlags.Walls),
        };
    }

    public void Initialize(PathFindingData_Level levelData)
    {
        if (initialized)
            Cleanup();

        initialized = true;
        pathFindingData = levelData;
        width = levelData.width;
        height = levelData.height;
        depth = levelData.depth;
        offset = levelData.offset;

        int totalCount = width * height * depth;
        pathFindingNodes = levelData.pathFindingNodes;
    }

    public void Initialize(LevelBoundsEntities levelBounds)
    {
        if (!createMapData)
            return;

        if (initialized)
            Cleanup();

        createMapData = false;
        initialized = true;
        width = (int)(levelBounds.size.x / DISTANCE_BETWEEN_NODES) + 1;
        height = (int)(levelBounds.size.y / DISTANCE_BETWEEN_NODES) + 1;
        depth = (int)(levelBounds.size.z / DISTANCE_BETWEEN_NODES) + 1;

        float worldWidth = width * DISTANCE_BETWEEN_NODES;
        float worldHeight = height * DISTANCE_BETWEEN_NODES;
        float worldDepth = depth * DISTANCE_BETWEEN_NODES;

        int totalCount = width * height * depth;

        float xOffset = levelBounds.center.x - worldWidth / 2;
        float yOffset = levelBounds.center.y - worldHeight / 2;
        float zOffset = levelBounds.center.z - worldDepth / 2;

        offset = new float3(xOffset, yOffset, zOffset);

        EntityArchetype archetype = EntityManager.CreateArchetype(typeof(PathFindingData_NodeInfo));
        NativeArray<Entity> entities = EntityManager.CreateEntity(archetype, totalCount, Allocator.TempJob);

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                for(int h = 0; h < depth; h++)
                {
                    int index = i * height * depth + j * depth + h;
                    Entity entity = entities[index];
                    EntityManager.SetComponentData(entity, new PathFindingData_NodeInfo { index = index });
                }
            }
        }
        entities.Dispose();

        EntityManager.AddComponentData(singletonEntity, new PathFindingTag_Initialize());

        pathFindingNodes = new NativeArray<PathFindingData_Node>(totalCount, Allocator.Persistent);
    }

    public void SetCreateMapData()
    {
        createMapData = true;
    }

    public void SaveToFile(SceneRecord record)
    {
        if (!Directory.Exists(LOCAL_DIRECTORY))
            return;

        PathFindingSaveData_Level saveData = new PathFindingSaveData_Level(pathFindingData);
        byte[] data = ByteSerializer.ConvertToByteArray_Unsafe(saveData);

        string saveFileDirectory = string.Format("{0}/{1}.{2}", LOCAL_DIRECTORY, record.SceneName, SAVE_FILE_EXTENTION);
        File.WriteAllBytes(saveFileDirectory, data);
    }

    public static string GetFileToLoad(SceneRecord record)
    {
        return string.Format("{0}/{1}", RESOURCE_DIRECTORY, record.SceneName);
    }

    private void Cleanup()
    {
        pathFindingNodes.Dispose();
    }

    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected unsafe override void OnUpdate()
    {
        EntityCommandBuffer commandBuffer = commandBufferSystem.CreateCommandBuffer();
        commandBuffer.RemoveComponent(singletonEntity, typeof(PathFindingTag_Initialize));

        JobHandle handle = new PathFindingJob_Initialize
        {
            offset = offset,
            width = width,
            height = height,
            depth = depth,
            wallCheckDistance = WALL_CHECK_DISTANCE,
            distanceBetweenNodes = DISTANCE_BETWEEN_NODES,
            shipSizeSampleStart = SHIP_SIZE_SAMPLE_START,
            shipSizeSampleReduction = SHIP_SIZE_SAMPLE_REDUCTION,
            collisionFilter = collisionFilter,
            physicsWorld = buildPhysicsWorld.PhysicsWorld,
            commandBuffer = commandBuffer.AsParallelWriter(),
            pathFindingNodes = pathFindingNodes,

        }.ScheduleParallel();
        handle.Complete();

        pathFindingData = new PathFindingData_Level()
        {
            width = width,
            height = height,
            depth = depth,
            offset = offset,
            pathFindingNodes = pathFindingNodes,
        };
        commandBufferSystem.AddJobHandleForProducer(handle);
        SaveToFile(SceneLoader.instance.currentScene);
    }

    protected override void OnDestroy()
    {
        if (initialized)
            Cleanup();
    }
}

[BurstCompile]
public unsafe partial struct PathFindingJob_Initialize : IJobEntity
{
    [ReadOnly] public float3 offset;
    [ReadOnly] public int width;
    [ReadOnly] public int height;
    [ReadOnly] public int depth;
    [ReadOnly] public float distanceBetweenNodes;
    [ReadOnly] public float shipSizeSampleStart;
    [ReadOnly] public float shipSizeSampleReduction;
    [ReadOnly] public float wallCheckDistance;
    [ReadOnly] public CollisionFilter collisionFilter;
    [ReadOnly] public PhysicsWorld physicsWorld;
    public EntityCommandBuffer.ParallelWriter commandBuffer;
    [NativeDisableParallelForRestriction] public NativeArray<PathFindingData_Node> pathFindingNodes;

    public void Execute(Entity entity, [EntityInQueryIndex] int entityIndex, in PathFindingData_NodeInfo nodeInfo)
    {
        int xIndex = (nodeInfo.index / depth / height) % width;
        int yIndex = (nodeInfo.index / depth) % height;
        int zIndex = nodeInfo.index % depth;
        float3 worldPosition = new float3(xIndex * distanceBetweenNodes, yIndex * distanceBetweenNodes, zIndex * distanceBetweenNodes) + offset;

        DistanceHit distanceHit;
        PointDistanceInput distanceInput = new PointDistanceInput
        {
            Filter = collisionFilter,
            MaxDistance = wallCheckDistance,
            Position = worldPosition,
        };

        float3 directionToWall = new float3();
        bool wallNearby;
        bool blocked;
        float shipSizeSampleInitial;
        if (physicsWorld.CalculateDistance(distanceInput, out distanceHit))
        {
            wallNearby = true;

            if (physicsWorld.CheckSphere(worldPosition, 0.1f, collisionFilter))
            {
                blocked = true;
                shipSizeSampleInitial = 0;
            }
            else
            {
                blocked = false;
                directionToWall = distanceHit.Position - worldPosition;
                shipSizeSampleInitial = distanceHit.Distance;
            }
        }
        else
        {
            wallNearby = false;
            blocked = false;
            shipSizeSampleInitial = shipSizeSampleStart;
        }


        PathFindingData_Node nodeData = new PathFindingData_Node
        {
            directionToWall = directionToWall,
            wallNearby = wallNearby,
            blocked = blocked,
        };

        int index = 0;
        if (!blocked)
        {
            for (int i = xIndex - 1; i <= xIndex + 1; i++)
            {
                for (int j = yIndex - 1; j <= yIndex + 1; j++)
                {
                    for (int h = zIndex - 1; h <= zIndex + 1; h++)
                    {
                        if (i == xIndex && j == yIndex && h == zIndex)
                            continue;

                        if (IsOutOfBounds(i, j, h))
                        {
                            nodeData.maxShipSizes[index++] = 0;
                            continue;
                        }

                        float3 difference = new float3(i - xIndex, j - yIndex, h - zIndex) * distanceBetweenNodes;
                        float distance = math.length(difference);
                        float3 direction = math.normalize(difference);

                        float shipSizeSample = shipSizeSampleInitial;
                        while (shipSizeSample > 0)
                        {
                            if (!physicsWorld.SphereCast(worldPosition, shipSizeSample, direction, distance, collisionFilter))
                                break;

                            shipSizeSample -= shipSizeSampleReduction;
                        }
                        shipSizeSample = math.max(0, shipSizeSample);
                        nodeData.maxShipSizes[index++] = shipSizeSample;
                    }
                }
            }
        }

        pathFindingNodes[nodeInfo.index] = nodeData;
        commandBuffer.DestroyEntity(entityIndex, entity);
    }

    private bool IsOutOfBounds(int xIndex, int yIndex, int zIndex)
    {
        if (xIndex < 0)
            return true;

        if (yIndex < 0)
            return true;

        if (zIndex < 0)
            return true;

        if (xIndex >= width)
            return true;

        if (yIndex >= height)
            return true;

        if (zIndex >= depth)
            return true;

        return false;
    }
}