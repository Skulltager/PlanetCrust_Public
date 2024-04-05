using SheetCodes;
using Unity.Entities;
using UnityEngine;

public class EnvironmentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private EnvironmentIdentifier identifier = default;
    private EnvironmentRecord record;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        record = identifier.GetRecord();

        dstManager.AddComponentData(entity, new EntityTag_Initialize { });
        dstManager.AddComponentData(entity, new EnvironmentEntityTag { });
        dstManager.AddComponentData(entity, new EntityData_Random { });

        dstManager.AddComponentData(entity, new EntityData_ImpactDamage
        {
            baseMinImpactDamage = record.BaseMinImpactDamage,
            baseMaxImpactDamage = record.BaseMaxImpactDamage,
            minVelocityImpactDamage = record.MinVelocityImpactDamage,
            maxVelocityImpactDamage = record.MaxVelocityImpactDamage,
            minVelocityImpactSpeed = record.MinVelocityImpactSpeed,
        });
    }
}