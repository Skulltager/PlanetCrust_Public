
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TrackerAuthoring_PostPhysics : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    [SerializeField] private GameObject trackerEntity = default;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        TrackerData_PostPhysics data = new TrackerData_PostPhysics
        {
            entity = conversionSystem.GetPrimaryEntity(trackerEntity),
        };

        dstManager.AddComponentData(entity, data);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(trackerEntity);
    }
}