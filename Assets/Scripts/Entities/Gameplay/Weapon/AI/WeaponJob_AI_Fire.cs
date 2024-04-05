using SheetCodes;
using System.Collections;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PostUpdateSystemGroup))]
public partial class WeaponSystem_AI_Fire : SystemBase
{ 
    private NativeQueue<WeaponEventData_Fire> weaponFireEvents;
    private EndFixedStepSimulationEntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        weaponFireEvents = new NativeQueue<WeaponEventData_Fire>(Allocator.Persistent);
        entityCommandBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnStartRunning()
    {
        this.RegisterPhysicsRuntimeSystemReadOnly();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new WeaponJob_AI_Fire
        {
            sceneIdentifier = SceneLoader.instance.currentSceneIdentifier,
            weaponFireEvents = weaponFireEvents.AsParallelWriter(),
            commandBuffer = entityCommandBufferSystem.CreateCommandBuffer().AsParallelWriter(),
            ref_Target = GetComponentDataFromEntity<ShipEntityData_AI_Target>(true),
            ref_Translation = GetComponentDataFromEntity<Translation>(true),
            ref_LocalToWorld = GetComponentDataFromEntity<LocalToWorld>(true),
            ref_PhysicsVelocity = GetComponentDataFromEntity<PhysicsVelocity>(true),
            elapsedTime = Time.ElapsedTime,
        }.ScheduleParallel();

        //handle.AddBuildPhysicsWorldDependencyToComplete();
        //entityCommandBufferSystem.AddJobHandleForProducer(handle);

        handle.Complete();

        while(weaponFireEvents.Count > 0)
            WeaponEventSystem_Fire.Trigger(weaponFireEvents.Dequeue());
    }

    protected override void OnDestroy()
    {
        weaponFireEvents.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(WeaponEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct WeaponJob_AI_Fire : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter commandBuffer;
        public NativeQueue<WeaponEventData_Fire>.ParallelWriter weaponFireEvents;
        [ReadOnly] public SceneIdentifier sceneIdentifier;
        [ReadOnly] public ComponentDataFromEntity<ShipEntityData_AI_Target> ref_Target;
        [ReadOnly] public ComponentDataFromEntity<Translation> ref_Translation;
        [ReadOnly] public ComponentDataFromEntity<LocalToWorld> ref_LocalToWorld;
        [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> ref_PhysicsVelocity;
        [ReadOnly] public double elapsedTime;

        public void Execute(
            [EntityInQueryIndex] int entityIndex,
            in DynamicBuffer<BufferElement_Reference_WeaponFireLocation> weaponFireLocations,
            ref EntityData_Random random,
            ref WeaponEntityData_Cooldown cooldown,
            in ShipEntityData_MainEntity mainEntity,
            in WeaponEntityData_Fire fire,
            in WeaponEntityData_Active active)
        {
            if (!active.value)
                return;

            if (elapsedTime < cooldown.value)
                return;

            ShipEntityData_AI_Target target = ref_Target[mainEntity.value];
            if (!ref_Translation.TryGetComponent(target.value, out Translation targetTranslation))
                return;

            PhysicsVelocity targetPhysicsVelocity = ref_PhysicsVelocity[target.value];

            for (int i = 0; i < weaponFireLocations.Length; i++)
            {
                LocalToWorld localToWorld = ref_LocalToWorld[weaponFireLocations[i].entity];
                float3 worldPosition = localToWorld.Position;
                quaternion worldRotation = localToWorld.Rotation;

                float3 forward = math.mul(worldRotation, math.forward());
                float3 up = math.mul(worldRotation, math.up());

                if (!math2.TryCalculateInterceptCourse(targetTranslation.Value, targetPhysicsVelocity.Linear, worldPosition, fire.projectileSpeed, out float3 desiredDirection))
                    return;

                weaponFireEvents.Enqueue(new WeaponEventData_Fire
                {
                    position = worldPosition,
                    rotation = worldRotation,
                });

                float3 fireDirection = math2.RotateTowards(forward, desiredDirection, fire.firingAngle * Mathf.Deg2Rad);

                for (int j = 0; j < fire.projectileCount; j++)
                {
                    float offset = random.value.NextFloat(0, fire.spread);
                    float angle = random.value.NextFloat(0, 360f);
                    random.value = new Unity.Mathematics.Random(random.value.NextUInt());

                    float3 fireRotationDirection = new float3(fireDirection.z, fireDirection.x, fireDirection.y);
                    float3 randomRotation = math2.RotateTowards(fireDirection, fireRotationDirection, offset * Mathf.Deg2Rad);
                    float3 finalDirection = math2.RotateRound(randomRotation, fireDirection, angle);
                    float3 velocity = finalDirection * fire.projectileSpeed;

                    Entity projectileEntity = commandBuffer.Instantiate(entityIndex, fire.projectilePrefab);
                    commandBuffer.SetComponent(entityIndex, projectileEntity, new ProjectileEntityData_Owner { value = mainEntity.value });
                    commandBuffer.SetComponent(entityIndex, projectileEntity, new Translation { Value = worldPosition });
                    commandBuffer.SetComponent(entityIndex, projectileEntity, new Rotation { Value = quaternion.LookRotation(finalDirection, up) });
                    commandBuffer.SetComponent(entityIndex, projectileEntity, new ProjectileEntityData_Velocity { value = velocity });
                    sceneIdentifier.AddSceneTag(commandBuffer, entityIndex, projectileEntity);
                }
            }
            cooldown.value = elapsedTime + fire.cooldown;
        }
    }
}