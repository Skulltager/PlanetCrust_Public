
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(PostUpdateSystemGroup))]
[UpdateBefore(typeof(WeaponSystem_AI_Fire))]
public partial class WeaponSystem_AI_EnableWithinAngleOfTarget : SystemBase
{
    public BuildPhysicsWorld buildPhysicsWorld;
    public CollisionFilter collisionFilter;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        collisionFilter = new CollisionFilter
        {
            BelongsTo = (uint)LayerFlags.Projectiles,
            CollidesWith = (uint)(LayerFlags.Walls),
        };
    }

    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new WeaponJob_AI_EnableWithinAngleOfTarget
        {
            ref_TargetPosition = GetComponentDataFromEntity<ShipEntityData_TargetPosition>(true),
            physicsWorld = buildPhysicsWorld.PhysicsWorld,
            collisionFilter = collisionFilter,
        }.ScheduleParallel();
        handle.AddBuildPhysicsWorldDependencyToComplete();
    }

    [BurstCompile]
    [WithAll(typeof(WeaponEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct WeaponJob_AI_EnableWithinAngleOfTarget : IJobEntity
    {
        [ReadOnly] public ComponentDataFromEntity<ShipEntityData_TargetPosition> ref_TargetPosition;
        [ReadOnly] public PhysicsWorld physicsWorld;
        public CollisionFilter collisionFilter;

        private void Execute(
            ref WeaponEntityData_Active active,
            in ShipEntityData_MainEntity mainEntity,
            in WeaponEntityData_AI_FiringAngle firingAngle,
            in LocalToWorld localToWorld)
        {
            ShipEntityData_TargetPosition targetPosition = ref_TargetPosition[mainEntity.value];
            RaycastInput rayCastInput = new RaycastInput
            {
                Start = localToWorld.Position,
                End = targetPosition.targetPosition,
                Filter = collisionFilter,
            };

            if (physicsWorld.CastRay(rayCastInput))
            {
                active.value = false;
                return;
            }

            float3 direction = math.normalize(targetPosition.targetPosition - localToWorld.Position);
            float3 forward = math.mul(localToWorld.Rotation, math.forward());
            float angleToTarget = math2.Angle(forward, direction);
            if (math.isnan(angleToTarget))
                angleToTarget = 0;

            active.value = angleToTarget < firingAngle.value;
        }
    }
}
