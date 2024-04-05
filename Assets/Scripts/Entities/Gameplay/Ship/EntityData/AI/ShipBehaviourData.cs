
using Unity.Entities;
using UnityEngine;

public abstract class ShipBehaviourData : MonoBehaviour
{
    public int priority;

    public abstract void AddComponentData(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem, int bufferIndex);
}