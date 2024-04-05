
using FMOD;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(CollisionEventSystemGroup))]
public partial class ShipSystem_Collision_ImpactDamage : SystemBase
{
    private PostPhysicsCommandBufferSystem postPhysicsCommandBufferSystem;
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        postPhysicsCommandBufferSystem = World.GetOrCreateSystem<PostPhysicsCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_Collision_ImpactDamage
        {
            commandBuffer = postPhysicsCommandBufferSystem.CreateCommandBuffer(),
            ref_ShipEntityTag = GetComponentDataFromEntity<ShipEntityTag>(true),
            ref_Resistances = GetComponentDataFromEntity<ShipEntityData_Resistances>(true),
            ref_EnvironmentEntityTag = GetComponentDataFromEntity<EnvironmentEntityTag>(true),
            ref_ImpactDamage = GetComponentDataFromEntity<EntityData_ImpactDamage>(true),
            ref_Random = GetComponentDataFromEntity<EntityData_Random>(false),
            physicsWorld = buildPhysicsWorld.PhysicsWorld,

        }.Schedule(stepPhysicsWorld.Simulation, Dependency);
        postPhysicsCommandBufferSystem.AddJobHandleForProducer(handle);
    }

    [BurstCompile]
    private struct ShipJob_Collision_ImpactDamage : ICollisionEventsJob
    {
        public EntityCommandBuffer commandBuffer;

        [ReadOnly] public PhysicsWorld physicsWorld;

        [ReadOnly] public ComponentDataFromEntity<ShipEntityTag> ref_ShipEntityTag;
        [ReadOnly] public ComponentDataFromEntity<ShipEntityData_Resistances> ref_Resistances;
        [ReadOnly] public ComponentDataFromEntity<EnvironmentEntityTag> ref_EnvironmentEntityTag;
        [ReadOnly] public ComponentDataFromEntity<EntityData_ImpactDamage> ref_ImpactDamage;

        public ComponentDataFromEntity<EntityData_Random> ref_Random;

        public void Execute(CollisionEvent collisionEvent)
        {
            bool isFirstShip = ref_ShipEntityTag.HasComponent(collisionEvent.EntityA);
            bool isSecondShip = ref_ShipEntityTag.HasComponent(collisionEvent.EntityB);

            if (!isFirstShip && !isSecondShip)
                return;

            if (isFirstShip && isSecondShip)
            {
                CollisionEvent.Details collisionDetials = collisionEvent.CalculateDetails(ref physicsWorld);
                HandleCollision(collisionDetials, collisionEvent.EntityA, collisionEvent.EntityB);
                HandleCollision(collisionDetials, collisionEvent.EntityB, collisionEvent.EntityA);
                return;
            }

            bool isFirstEnvironment = ref_EnvironmentEntityTag.HasComponent(collisionEvent.EntityA);

            if (isFirstEnvironment)
            {
                CollisionEvent.Details collisionDetials = collisionEvent.CalculateDetails(ref physicsWorld);
                HandleCollision(collisionDetials, collisionEvent.EntityB, collisionEvent.EntityA);
                return;
            }

            bool isSecondEnvironment = ref_EnvironmentEntityTag.HasComponent(collisionEvent.EntityB);
            if (isSecondEnvironment)
            {
                CollisionEvent.Details collisionDetials = collisionEvent.CalculateDetails(ref physicsWorld);
                HandleCollision(collisionDetials, collisionEvent.EntityA, collisionEvent.EntityB);
                return;
            }
        }

        private void HandleCollision(CollisionEvent.Details details, Entity ship, Entity collisionObject)
        {
            EntityData_ImpactDamage impactDamage = ref_ImpactDamage[collisionObject];
            if (details.EstimatedImpulse < impactDamage.minVelocityImpactSpeed)
                return;

            ShipEntityData_Resistances resistances = ref_Resistances[ship];
            EntityData_Random random = ref_Random[collisionObject];

            int baseDamage = random.value.NextInt(impactDamage.baseMinImpactDamage, impactDamage.baseMaxImpactDamage);
            int velocityDamage = (int)(random.value.NextFloat(impactDamage.minVelocityImpactDamage, impactDamage.maxVelocityImpactDamage) * details.EstimatedImpulse);

            int totalDamage = baseDamage + velocityDamage - resistances.impactResistance;
            if (totalDamage <= 0)
            {
                random.value = new Unity.Mathematics.Random(random.value.NextUInt());
                ref_Random[collisionObject] = random;
                return;
            }

            Entity damageEntity = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(damageEntity, new ShipEntityData_TakeDamage
            {
                damageType = DamageType.Impact,
                amount = totalDamage,
                ship = ship,
            });

            random.value = new Unity.Mathematics.Random(random.value.NextUInt());
            ref_Random[collisionObject] = random;
        }
    }
}