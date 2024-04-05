
using Unity.Entities;
using UnityEngine;

public class EntityPrefab : MonoBehaviour
{
    [SerializeField] private GameObject prefab = default;
    public Entity entity { private set; get; }

    private BlobAssetStore blobAssetStore;

    private void Awake()
    {
        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
        entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }
}