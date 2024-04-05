using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(PreUpdateSystemGroup))]
public partial class ProjectileSystem_Move : SystemBase
{
    private BuildPhysicsWorld buildPhysicsWorld;
    private NativeQueue<ProjectileHitEventData> wallHitEventQueue;
    private NativeQueue<ProjectileHitEventData> shipHitEventQueue;
    private CollisionFilter collisionFilter;

    private DestroyCommandBufferSystem destroyCommandBufferSystem;
    private PostPhysicsCommandBufferSystem postPhysicsCommandBufferSystem;

    public event Action<ProjectileHitEventData> onProjectileHitWall;
    public event Action<ProjectileHitEventData> onProjectileHitShip;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        destroyCommandBufferSystem = World.GetOrCreateSystem<DestroyCommandBufferSystem>();
        postPhysicsCommandBufferSystem = World.GetOrCreateSystem<PostPhysicsCommandBufferSystem>();
        wallHitEventQueue = new NativeQueue<ProjectileHitEventData>(Allocator.Persistent);
        shipHitEventQueue = new NativeQueue<ProjectileHitEventData>(Allocator.Persistent);
        collisionFilter = new CollisionFilter()
        {
            BelongsTo = (uint)LayerFlags.Projectiles,
            CollidesWith = (uint)(LayerFlags.Ships_Detail | LayerFlags.Walls),
        };
    }

    protected override void OnUpdate()
    {
        NativeQueue<ProjectileHitEventData>.ParallelWriter wallHitWriter = wallHitEventQueue.AsParallelWriter();
        NativeQueue<ProjectileHitEventData>.ParallelWriter shipHitWriter = shipHitEventQueue.AsParallelWriter();

        JobHandle handle = new ProjectileJob_Move
        {
            destroyCommandBufferSystem = destroyCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
            postPhysicsCommandBufferSystem = postPhysicsCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
            collisionFilter = collisionFilter,
            wallHitEvents = wallHitWriter,
            shipHitEvents = shipHitWriter,
            fixedDeltaTime = Time.DeltaTime,
            physicsWorld = buildPhysicsWorld.PhysicsWorld,
            ref_ColliderReference = GetComponentDataFromEntity<EntityColliderReference>(true),
            ref_MainEntity = GetComponentDataFromEntity<ShipEntityData_MainEntity>(true),
        }.ScheduleParallel();

        destroyCommandBufferSystem.AddJobHandleForProducer(handle);
        postPhysicsCommandBufferSystem.AddJobHandleForProducer(handle);

        handle.Complete();
        while (wallHitEventQueue.TryDequeue(out ProjectileHitEventData projectileHitEventData))
        {
            if (onProjectileHitWall != null)
                onProjectileHitWall(projectileHitEventData);
        }

        while (shipHitEventQueue.TryDequeue(out ProjectileHitEventData projectileHitEventData))
        {
            if (onProjectileHitShip != null)
                onProjectileHitShip(projectileHitEventData);
        }
    }

    protected override void OnDestroy()
    {
        wallHitEventQueue.Dispose();
        shipHitEventQueue.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(ProjectileEntityTag))]
    private partial struct ProjectileJob_Move : IJobEntity
    {
        public NativeQueue<ProjectileHitEventData>.ParallelWriter wallHitEvents;
        public NativeQueue<ProjectileHitEventData>.ParallelWriter shipHitEvents;
        public EntityCommandBuffer.ParallelWriter destroyCommandBufferSystem;
        public EntityCommandBuffer.ParallelWriter postPhysicsCommandBufferSystem;

        [ReadOnly] public ComponentDataFromEntity<EntityColliderReference> ref_ColliderReference;
        [ReadOnly] public ComponentDataFromEntity<ShipEntityData_MainEntity> ref_MainEntity;
        [ReadOnly] public CollisionFilter collisionFilter;
        [ReadOnly] public PhysicsWorld physicsWorld;
        [ReadOnly] public float fixedDeltaTime;

        private void Execute(
            Entity entity,
            [EntityInQueryIndex] int entityIndex,
            ref Translation translation,
            ref ProjectileEntityData_Life life,
            in EntityData_Random random,
            in ProjectileEntityData_Damage damage,
            in ProjectileEntityData_Owner owner,
            in ProjectileEntityData_Velocity velocity)
        {
            float3 startPosition = translation.Value;
            float3 moveDistance = velocity.value * fixedDeltaTime;
            float moveAmount = math.length(moveDistance);
            bool endOfLifetime = false;

            if (life.distanceRemaining < moveAmount)
            {
                moveDistance = math.normalize(velocity.value) * life.distanceRemaining;
                endOfLifetime = true;
            }
            else
                life.distanceRemaining -= moveAmount;

            if (life.timeRemaining < fixedDeltaTime)
            {
                moveDistance = moveAmount * (life.timeRemaining / fixedDeltaTime);
                endOfLifetime = true;
            }
            else
                life.timeRemaining -= fixedDeltaTime;

            float3 endPosition = startPosition + moveDistance;
            if (Raycast(owner.value, startPosition, endPosition, out RaycastHit hit))
            {
                ref_ColliderReference.TryGetComponent(hit.Entity, out EntityColliderReference colliderReference);
                ProjectileHitEventData eventData = new ProjectileHitEventData()
                {
                    normalHit = hit.SurfaceNormal,
                    point = hit.Position,
                };

                switch (colliderReference.referenceType)
                {
                    case ColliderReferenceType.Wall:
                        wallHitEvents.Enqueue(eventData);
                        break;
                    case ColliderReferenceType.Ship:
                        Entity damageEntity = postPhysicsCommandBufferSystem.CreateEntity(entityIndex);
                        postPhysicsCommandBufferSystem.AddComponent(entityIndex, damageEntity, new ShipEntityData_TakeDamage
                        {
                            damageType = damage.damageType,
                            amount = random.value.NextInt(damage.minDamage, damage.maxDamage),
                            ship = ref_MainEntity[hit.Entity].value,
                        });
                        shipHitEvents.Enqueue(eventData);
                        break;
                }
                destroyCommandBufferSystem.AddComponent<EntityTag_Destroy>(entityIndex, entity);
            }
            else
            {
                if (endOfLifetime)
                    destroyCommandBufferSystem.AddComponent<EntityTag_Destroy>(entityIndex, entity);

                translation.Value += velocity.value * fixedDeltaTime;
            }
        }

        private bool Raycast(Entity owner, float3 fromPosition, float3 toPosition, out RaycastHit hit)
        {
            RaycastInput ray = new RaycastInput()
            {
                Start = fromPosition,
                End = toPosition,
                Filter = collisionFilter,
            };
            NativeList<RaycastHit> allHits = new NativeList<RaycastHit>(Allocator.Temp);
            if (physicsWorld.CastRay(ray, ref allHits))
            {
                for (int i = 0; i < allHits.Length; i++)
                {
                    hit = allHits[i];
                    EntityColliderReference colliderReference = ref_ColliderReference[hit.Entity];
                    if (colliderReference.referenceType != ColliderReferenceType.Ship)
                    {
                        allHits.Dispose();
                        return true;
                    }

                    Entity shipMain = ref_MainEntity[hit.Entity].value;
                    if (shipMain == owner)
                        continue;

                    allHits.Dispose();
                    return true;
                }
            }
            hit = default;
            allHits.Dispose();
            return false;
        }
    }

}
