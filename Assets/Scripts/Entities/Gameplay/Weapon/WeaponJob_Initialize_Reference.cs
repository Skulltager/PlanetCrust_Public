
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializeSystemGroup))]
[UpdateAfter(typeof(ShipSystem_Initialize_Reference))]
public partial class WeaponSystem_Initialize_Reference : SystemBase
{
    protected override void OnUpdate()
    {
        new WeaponJob_Initialize_Reference()
        {
            entityManager = EntityManager,
            ref_LocalToWorld = GetComponentDataFromEntity<LocalToWorld>(),
        }.Run();
    }

    [WithAll(typeof(WeaponEntityTag), typeof(EntityTag_Initialize))]
    private partial struct WeaponJob_Initialize_Reference : IJobEntity
    {
        public EntityManager entityManager;
        [ReadOnly] public ComponentDataFromEntity<LocalToWorld> ref_LocalToWorld;

        private void Execute(
            Entity entity,
            in DynamicBuffer<BufferElement_Reference_WeaponFireLocation> weaponFireLocations,
            in WeaponEntityData_Fire fire,
            in ShipEntityData_MainEntity mainEntity,
            WeaponEntityReference reference)
        {
            ShipEntityReference shipEntityReference = entityManager.GetComponentData<ShipEntityReference>(mainEntity.value);
            WeaponTransform[] weaponTransforms = new WeaponTransform[weaponFireLocations.Length];
            for (int i = 0; i < weaponFireLocations.Length; i++)
            {
                LocalToWorld localToWorld = ref_LocalToWorld[weaponFireLocations[i].entity];
                weaponTransforms[i] = new WeaponTransform
                {
                    position = localToWorld.Position,
                    rotation = localToWorld.Rotation,
                };
            }
            reference.value = new WeaponData(shipEntityReference.value, entity, weaponTransforms);
        }
    }
}