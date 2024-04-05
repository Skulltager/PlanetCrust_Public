using SheetCodes;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class BowlEntitySpawner : LevelPreparation
{
    [SerializeField] private int seed = default;
    [SerializeField] private int spawnCount = default;
    [SerializeField] private EntityPrefab bowlPrefab = default;

    public override void Initialize(LevelBoundsEntities levelBounds)
    {
        NativeArray<Entity> entities = new NativeArray<Entity>(spawnCount, Allocator.TempJob);
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.Instantiate(bowlPrefab.entity, entities);

        SceneIdentifier sceneIdentifier = SceneLoader.instance.currentSceneIdentifier;
        System.Random random = new System.Random(seed);
        for (int i = 0; i < spawnCount; i++)
        {
            float xPosition = ((float)random.NextDouble()) * levelBounds.size.x + levelBounds.min.x;
            float yPosition = ((float)random.NextDouble()) * levelBounds.size.y + levelBounds.min.y;
            float zPosition = ((float)random.NextDouble()) * levelBounds.size.z + levelBounds.min.z;
            float xRotation = ((float)random.NextDouble()) * 360;
            float yRotation = ((float)random.NextDouble()) * 360;
            float zRotation = ((float)random.NextDouble()) * 360;
            Vector3 spawnPosition = new Vector3(xPosition, yPosition, zPosition);
            Quaternion spawnRotation = Quaternion.Euler(xRotation, yRotation, zRotation);
            Entity entity = entities[i];
            entityManager.SetName(entity, "Bowl");
            entityManager.SetComponentData(entity, new Translation { Value = spawnPosition });
            entityManager.SetComponentData(entity, new Rotation { Value = spawnRotation });


            sceneIdentifier.AddSceneTag(entityManager, entity);
        }
        entities.Dispose();

        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        buildPhysicsWorld.Update();
    }
}