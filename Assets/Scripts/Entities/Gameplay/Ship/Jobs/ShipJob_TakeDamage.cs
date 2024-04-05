
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(PostUpdateSystemGroup))]
public partial class ShipSystem_TakeDamage : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem commandBufferSystem;
    private NativeQueue<ShipEventDataIndex_TakeDamage> takeDamageEvents;

    protected override void OnCreate()
    {
        commandBufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        takeDamageEvents = new NativeQueue<ShipEventDataIndex_TakeDamage>(Allocator.Persistent);
    }

    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_TakeDamage
        {
            takeDamageEvents = takeDamageEvents,
            ref_Health = GetComponentDataFromEntity<ShipEntityData_Health>(false),
            ref_ReferenceIndex = GetComponentDataFromEntity<ShipEntityReferenceIndex>(true),
            commandBuffer = commandBufferSystem.CreateCommandBuffer(),
        }.Schedule();
        commandBufferSystem.AddJobHandleForProducer(handle);
        handle.Complete();

        while (takeDamageEvents.Count > 0)
        {
            ShipEventDataIndex_TakeDamage takeDamageEvent = takeDamageEvents.Dequeue();
            ShipData shipData = ShipData.allShipDatas[takeDamageEvent.index];
            shipData.TriggerOnTakeDamage(takeDamageEvent.eventData);
        }
    }

    protected override void OnDestroy()
    {
        takeDamageEvents.Dispose();
    }

    [BurstCompile]
    private partial struct ShipJob_TakeDamage : IJobEntity
    {
        public NativeQueue<ShipEventDataIndex_TakeDamage> takeDamageEvents;
        public EntityCommandBuffer commandBuffer;
        public ComponentDataFromEntity<ShipEntityData_Health> ref_Health;
        [ReadOnly] public ComponentDataFromEntity<ShipEntityReferenceIndex> ref_ReferenceIndex;

        private void Execute(
            Entity entity,
            in ShipEntityData_TakeDamage takeDamage)
        {
            ShipEntityData_Health health = ref_Health[takeDamage.ship];
            ShipEntityReferenceIndex referenceIndex = ref_ReferenceIndex[takeDamage.ship];
            switch (takeDamage.damageType)
            {
                case DamageType.Impact:
                    TakeDamage_Impact(referenceIndex.value, ref health, takeDamage);
                    break;
                case DamageType.Physical:
                case DamageType.Lightning:
                case DamageType.Corrosive:
                case DamageType.Fire:
                    TakeDamage_Physical(referenceIndex.value, ref health, takeDamage);
                    break;
            }
            ref_Health[takeDamage.ship] = health;
            commandBuffer.DestroyEntity(entity);
        }

        private void TakeDamage_Impact(int referenceIndex, ref ShipEntityData_Health health, ShipEntityData_TakeDamage takeDamage)
        {
            int damage = takeDamage.amount;
            if (damage <= 0)
                return;

            int armorDamage = math.min(health.armor, damage);
            damage -= armorDamage;
            health.armor -= armorDamage;

            int hullDamage = math.min(health.hull, damage);
            health.hull -= hullDamage;

            takeDamageEvents.Enqueue(new ShipEventDataIndex_TakeDamage
            {
                index = referenceIndex,
                eventData = new ShipEventData_TakeDamage
                {
                    armorDamage = armorDamage,
                    hullDamage = hullDamage,
                }
            });
        }

        private void TakeDamage_Physical(int referenceIndex, ref ShipEntityData_Health health, ShipEntityData_TakeDamage takeDamage)
        {
            int damage = takeDamage.amount;

            int shieldDamage = math.min(health.shield, damage);
            damage -= shieldDamage;
            health.shield -= shieldDamage;

            int armorDamage = math.min(health.armor, damage);
            damage -= armorDamage;
            health.armor -= armorDamage;

            int hullDamage = math.min(health.hull, damage);
            health.hull -= hullDamage;

            takeDamageEvents.Enqueue(new ShipEventDataIndex_TakeDamage
            {
                index = referenceIndex,
                eventData = new ShipEventData_TakeDamage
                {
                    armorDamage = armorDamage,
                    shieldDamage = shieldDamage,
                    hullDamage = hullDamage,
                }
            });
        }
    }
}
