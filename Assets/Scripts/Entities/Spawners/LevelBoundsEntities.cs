
using SheetCodes;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class LevelBoundsEntities : MonoBehaviour
{
    [SerializeField] private EntityPrefab wallPrefab = default;
    [SerializeField] private UnityEngine.BoxCollider boxCollider = default;
    [SerializeField] private float borderSize = default;
    private List<BlobAssetReference<Unity.Physics.Collider>> blobAssetReferences;

    public Vector3 min { private set; get; }
    public Vector3 max { private set; get; }
    public Vector3 size => max - min;
    public Vector3 center => (min + max) / 2;
    public float halfBorderSize => borderSize / 2;

    private void Awake()
    {
        blobAssetReferences = new List<BlobAssetReference<Unity.Physics.Collider>>();
    }

    public void Initialize()
    { 
        min = this.boxCollider.bounds.min;
        max = this.boxCollider.bounds.max;
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        NativeArray<Entity> entityList = new NativeArray<Entity>(6, Allocator.TempJob);
        entityManager.Instantiate(wallPrefab.entity, entityList);
        
        // Top
        Entity entity = entityList[0];
        entityManager.SetName(entity, "Top Wall");
        Vector3 position = new Vector3(center.x, max.y + halfBorderSize, center.z);
        Vector3 scale = new Vector3(size.x + borderSize * 2, borderSize, size.z + borderSize * 2);
        SceneIdentifier sceneIdentifier = SceneLoader.instance.currentSceneIdentifier;
        
        entityManager.SetComponentData(entity, new Translation { Value = position });
        entityManager.AddComponentData(entity, new CompositeScale { Value = float4x4.Scale(scale) });
        sceneIdentifier.AddSceneTag(entityManager, entity);

        BoxGeometry geometry = new BoxGeometry()
        {
            BevelRadius = halfBorderSize,
            Size = scale,
            Center = Vector3.zero,
            Orientation = Quaternion.identity,
        }; 
        BlobAssetReference<Unity.Physics.Collider> boxCollider = Unity.Physics.BoxCollider.Create(geometry);
        blobAssetReferences.Add(boxCollider);
        PhysicsCollider physicsCollider = new PhysicsCollider { Value = boxCollider };
        entityManager.SetComponentData(entity, physicsCollider);

        // Bottom
        entity = entityList[1];
        entityManager.SetName(entity, "Bottom Wall");
        
        position = new Vector3(center.x, min.y - halfBorderSize, center.z);
        scale = new Vector3(size.x + borderSize * 2, borderSize, size.z + borderSize * 2);
        
        entityManager.SetComponentData(entity, new Translation { Value = position });
        entityManager.AddComponentData(entity, new CompositeScale { Value = float4x4.Scale(scale) });
        sceneIdentifier.AddSceneTag(entityManager, entity);

        geometry = new BoxGeometry()
        {
            BevelRadius = halfBorderSize,
            Size = scale,
            Center = Vector3.zero,
            Orientation = Quaternion.identity,
        };
        boxCollider = Unity.Physics.BoxCollider.Create(geometry);
        blobAssetReferences.Add(boxCollider);
        physicsCollider = new PhysicsCollider { Value = boxCollider };
        entityManager.SetComponentData(entity, physicsCollider);
        
        // Left
        entity = entityList[2];
        entityManager.SetName(entity, "Left Wall");
        position = new Vector3(min.x - halfBorderSize, center.y, center.z);
        scale = new Vector3(borderSize, size.y, size.z + borderSize * 2);
        
        entityManager.SetComponentData(entity, new Translation { Value = position });
        entityManager.AddComponentData(entity, new CompositeScale { Value = float4x4.Scale(scale) });
        sceneIdentifier.AddSceneTag(entityManager, entity);

        geometry = new BoxGeometry()
        {
            BevelRadius = halfBorderSize,
            Size = scale,
            Center = Vector3.zero,
            Orientation = Quaternion.identity,
        };
        boxCollider = Unity.Physics.BoxCollider.Create(geometry);
        blobAssetReferences.Add(boxCollider);
        physicsCollider = new PhysicsCollider { Value = boxCollider };
        entityManager.SetComponentData(entity, physicsCollider);
        
        // Right
        entity = entityList[3];
        entityManager.SetName(entity, "Right Wall");
        position = new Vector3(max.x + halfBorderSize, center.y, center.z);
        scale = new Vector3(borderSize, size.y, size.z + borderSize * 2);
        
        entityManager.SetComponentData(entity, new Translation { Value = position });
        entityManager.AddComponentData(entity, new CompositeScale { Value = float4x4.Scale(scale) });
        sceneIdentifier.AddSceneTag(entityManager, entity);

        geometry = new BoxGeometry()
        {
            BevelRadius = halfBorderSize,
            Size = scale,
            Center = Vector3.zero,
            Orientation = Quaternion.identity,
        };
        boxCollider = Unity.Physics.BoxCollider.Create(geometry);
        blobAssetReferences.Add(boxCollider);
        physicsCollider = new PhysicsCollider { Value = boxCollider };
        entityManager.SetComponentData(entity, physicsCollider);
        
        // Front
        entity = entityList[4];
        entityManager.SetName(entity, "Front Wall");
        position = new Vector3(center.x, center.y, min.z - halfBorderSize);
        scale = new Vector3(size.x, size.y, borderSize);
        
        entityManager.SetComponentData(entity, new Translation { Value = position });
        entityManager.AddComponentData(entity, new CompositeScale { Value = float4x4.Scale(scale) });
        sceneIdentifier.AddSceneTag(entityManager, entity);

        geometry = new BoxGeometry()
        {
            BevelRadius = halfBorderSize,
            Size = scale,
            Center = Vector3.zero,
            Orientation = Quaternion.identity,
        };
        boxCollider = Unity.Physics.BoxCollider.Create(geometry);
        blobAssetReferences.Add(boxCollider);
        physicsCollider = new PhysicsCollider { Value = boxCollider };
        entityManager.SetComponentData(entity, physicsCollider);
        
        // Back
        entity = entityList[5];
        entityManager.SetName(entity, "Back Wall");
        position = new Vector3(center.x, center.y, max.z + halfBorderSize);
        scale = new Vector3(size.x, size.y, borderSize);
        
        entityManager.SetComponentData(entity, new Translation { Value = position });
        entityManager.AddComponentData(entity, new CompositeScale { Value = float4x4.Scale(scale) });
        sceneIdentifier.AddSceneTag(entityManager, entity);

        geometry = new BoxGeometry()
        {
            BevelRadius = halfBorderSize,
            Size = scale,
            Center = Vector3.zero,
            Orientation = Quaternion.identity,
        };
        boxCollider = Unity.Physics.BoxCollider.Create(geometry);
        blobAssetReferences.Add(boxCollider);
        physicsCollider = new PhysicsCollider { Value = boxCollider };
        entityManager.SetComponentData(entity, physicsCollider);

        entityList.Dispose();
    }

    private void OnDestroy()
    {
        foreach (BlobAssetReference<Unity.Physics.Collider> blobAssetReference in blobAssetReferences)
            blobAssetReference.Dispose();
    
        blobAssetReferences.Clear();
    }
}