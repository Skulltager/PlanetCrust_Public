using SheetCodes;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(PostUpdateSystemGroup))]
public partial class WeaponSystem_Player_Fire : SystemBase
{
    private NativeQueue<WeaponEventData_Fire> weaponFireEvents;
    private BeginInitializationEntityCommandBufferSystem entityCommandBufferSystem;

    protected override void OnCreate()
    {
        weaponFireEvents = new NativeQueue<WeaponEventData_Fire>(Allocator.Persistent);
        entityCommandBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new WeaponJob_Player_Fire
        {
            sceneIdentifier = SceneLoader.instance.currentSceneIdentifier,
            weaponFireEvents = weaponFireEvents.AsParallelWriter(),
            commandBuffer = entityCommandBufferSystem.CreateCommandBuffer(),
            ref_Energy = GetComponentDataFromEntity<ShipEntityData_Player_Energy>(false),

            ref_TargetPosition = GetComponentDataFromEntity<ShipEntityData_TargetPosition>(true),
            ref_LocalToWorld = GetComponentDataFromEntity<LocalToWorld>(true),
            elapsedTime = Time.ElapsedTime,
        }.Schedule();

        //entityCommandBufferSystem.AddJobHandleForProducer(handle);
        handle.Complete();

        while (weaponFireEvents.Count > 0)
            WeaponEventSystem_Fire.Trigger(weaponFireEvents.Dequeue());
    }

    protected override void OnDestroy()
    {
        weaponFireEvents.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(WeaponEntityTag), typeof(EntityTag_PlayerControlled))]
    private partial struct WeaponJob_Player_Fire : IJobEntity
    {
        public EntityCommandBuffer commandBuffer;
        public NativeQueue<WeaponEventData_Fire>.ParallelWriter weaponFireEvents;
        public ComponentDataFromEntity<ShipEntityData_Player_Energy> ref_Energy;

        [ReadOnly] public SceneIdentifier sceneIdentifier;
        [ReadOnly] public ComponentDataFromEntity<ShipEntityData_TargetPosition> ref_TargetPosition;
        [ReadOnly] public ComponentDataFromEntity<LocalToWorld> ref_LocalToWorld;
        [ReadOnly] public double elapsedTime;

        public void Execute(
            ref EntityData_Random random,
            ref WeaponEntityData_Cooldown cooldown,
            ref WeaponEntityData_Fire fire,
            in DynamicBuffer<BufferElement_Reference_WeaponFireLocation> weaponFireLocations,
            in ShipEntityData_MainEntity mainEntity,
            in WeaponEntityData_Active active,
            in WeaponEntityData_Player_EnergyCost energyCost)
        {
            if (!active.value)
                return;

            if (elapsedTime < cooldown.value)
                return;

            ShipEntityData_Player_Energy energy = ref_Energy[mainEntity.value];
            if (energy.energy < energyCost.value)
                return;

            for (int i = 0; i < weaponFireLocations.Length; i++)
            {
                LocalToWorld localToWorld = ref_LocalToWorld[weaponFireLocations[i].entity];
                float3 worldPosition = localToWorld.Position;
                quaternion worldRotation = localToWorld.Rotation;
                weaponFireEvents.Enqueue(new WeaponEventData_Fire
                {
                    position = worldPosition,
                    rotation = worldRotation,
                });

                float3 forward = math.mul(worldRotation, math.forward());
                float3 up = math.mul(worldRotation, math.up());
                ShipEntityData_TargetPosition targetPosition = ref_TargetPosition[mainEntity.value];
                float3 desiredDirection = math.normalize(targetPosition.targetPosition - worldPosition);

                float3 fireDirection = math2.RotateTowards(forward, desiredDirection, fire.firingAngle * Mathf.Deg2Rad);

                float offset = random.value.NextFloat(0, fire.spread);
                float angle = random.value.NextFloat(0, 360f);
                random.value = new Unity.Mathematics.Random(random.value.NextUInt());
                float3 fireRotationDirection = new float3(fireDirection.z, fireDirection.x, fireDirection.y);
                float3 randomRotation = math2.RotateTowards(fireDirection, fireRotationDirection, offset * Mathf.Deg2Rad);
                float3 finalDirection = math2.RotateRound(randomRotation, fireDirection, angle);
                float3 velocity = finalDirection * fire.projectileSpeed;

                Entity projectileEntity = commandBuffer.Instantiate(fire.projectilePrefab);

                commandBuffer.SetComponent(projectileEntity, new ProjectileEntityData_Owner { value = mainEntity.value });
                commandBuffer.SetComponent(projectileEntity, new Translation { Value = worldPosition });
                commandBuffer.SetComponent(projectileEntity, new Rotation { Value = quaternion.LookRotation(finalDirection, up) });
                commandBuffer.SetComponent(projectileEntity, new ProjectileEntityData_Velocity { value = velocity });
                sceneIdentifier.AddSceneTag(commandBuffer, projectileEntity);
            }
            energy.energy -= energyCost.value;
            ref_Energy[mainEntity.value] = energy;

            cooldown.value = elapsedTime + fire.cooldown;
        }
    }
}