using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShipAuthoring_RootEntity : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [SerializeField] private GameObject value = default;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        ShipEntityData_RootEntity data = new ShipEntityData_RootEntity
        {
            value = conversionSystem.GetPrimaryEntity(value),
        };

        dstManager.AddComponentData(entity, data);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(value);
    }
}