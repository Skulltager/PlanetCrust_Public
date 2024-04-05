using SheetCodes;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public abstract class WeaponAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField] private GameObject[] weaponFiringLocations = default;

    protected abstract float cooldown { get; }
    protected abstract float angle { get; }
    protected abstract float spread { get; }
    protected abstract float projectileSpeed { get; }
    protected abstract int projectileCount { get; }
    protected abstract Entity projectilePrefab { get; }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        ConvertSub(entity, dstManager, conversionSystem);

        DynamicBuffer<BufferElement_Reference_WeaponFireLocation> dynamicBuffer = dstManager.AddBuffer<BufferElement_Reference_WeaponFireLocation>(entity);
        foreach (GameObject weaponFiringLocation in weaponFiringLocations)
            dynamicBuffer.Add(conversionSystem.GetPrimaryEntity(weaponFiringLocation));

        dstManager.AddComponentData(entity, new EntityTag_Initialize { });
        dstManager.AddComponentData(entity, new EntityData_Random { });

        dstManager.AddComponentData(entity, new WeaponEntityTag { });
        dstManager.AddComponentData(entity, new WeaponEntityReference { });
        dstManager.AddComponentData(entity, new WeaponEntityData_Cooldown { value = 0 });
        dstManager.AddComponentData(entity, new WeaponEntityData_Active { value = false });
        dstManager.AddComponentData(entity, new WeaponEntityData_Fire
        {
            cooldown = cooldown,
            firingAngle = angle,
            spread = spread,
            projectileCount = projectileCount,
            projectilePrefab = projectilePrefab,
            projectileSpeed = projectileSpeed,
        });
    }

    public abstract void ConvertSub(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem);

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(weaponFiringLocations);
    }
}