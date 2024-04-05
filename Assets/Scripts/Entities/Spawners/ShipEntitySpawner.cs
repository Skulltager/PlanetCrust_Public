using SheetCodes;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class ShipEntitySpawner : LevelPreparation
{
    [SerializeField] private int seed = default;
    [SerializeField] private int team = default;
    [SerializeField] private int spawnCount = default;
    [SerializeField] private AiShipIdentifier identifier = default;

    public override void Initialize(LevelBoundsEntities levelBounds)
    {
        AiShipRecord record = identifier.GetRecord();
        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        NativeArray<Entity> entities = new NativeArray<Entity>(spawnCount, Allocator.TempJob);
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.Instantiate(record.GetEntity(), entities);
        System.Random random = new System.Random(seed);

        SceneIdentifier sceneIdentifier = SceneLoader.instance.currentSceneIdentifier;

        float3 rootPosition = float3.zero;
        quaternion rootRotation = quaternion.identity;

        for (int i = 0; i < spawnCount; i++)
        {
            float3 position;
            while (true)
            {
                position.x = ((float)random.NextDouble()) * levelBounds.size.x + levelBounds.min.x;
                position.y = ((float)random.NextDouble()) * levelBounds.size.y + levelBounds.min.y;
                position.z = ((float)random.NextDouble()) * levelBounds.size.z + levelBounds.min.z;

                CollisionFilter filter = new CollisionFilter()
                {
                    BelongsTo = (uint)LayerFlags.Ships,
                    CollidesWith = (uint)(LayerFlags.Ships | LayerFlags.Walls),
                };

                if (buildPhysicsWorld.PhysicsWorld.CheckSphere(position, record.Radius, filter))
                    continue;

                break;
            }

            float xRotation = ((float)random.NextDouble()) * 360;
            float yRotation = ((float)random.NextDouble()) * 360;
            float zRotation = ((float)random.NextDouble()) * 360;
            Quaternion rotation = Quaternion.Euler(xRotation, yRotation, zRotation);
            Entity entity = entities[i];
            entityManager.SetName(entity, "Ship");

            entityManager.SetComponentData(entity, new Translation { Value = position });
            entityManager.SetComponentData(entity, new Rotation { Value = rotation });
            sceneIdentifier.AddSceneTag(entityManager, entity);

            Entity mainEntity = entityManager.GetComponentData<ShipEntityData_MainEntity>(entity).value;

            Translation positionComponent = entityManager.GetComponentData<Translation>(mainEntity);
            Rotation rotationComponent = entityManager.GetComponentData<Rotation>(mainEntity);

            float3 localPosition = positionComponent.Value - rootPosition;
            quaternion rootRelativeRotation = math.mul(math.inverse(rootRotation), rotation);
            float3 rotatedLocalPosition = math.mul(rootRelativeRotation, localPosition);

            float3 rootOffset = position - rootPosition;
            float3 finalPosition = rootPosition + rotatedLocalPosition + rootOffset;

            quaternion finalRotation = math.mul(rotationComponent.Value, rootRelativeRotation);

            positionComponent.Value = finalPosition;
            rotationComponent.Value = finalRotation;

            entityManager.SetComponentData(mainEntity, positionComponent);
            entityManager.SetComponentData(mainEntity, rotationComponent);
            entityManager.SetComponentData(mainEntity, new ShipEntityData_Team { value = team });

            entityManager.SetName(mainEntity, "rigidbody");
            buildPhysicsWorld.Update();
        }

        entities.Dispose();
    }
}