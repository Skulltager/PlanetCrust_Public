
using SheetCodes;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public class PlayerEntitySpawner : LevelPreparation
{
    [SerializeField] private int seed = default;
    [SerializeField] private int team = default;
    [SerializeField] private PlayerShipIdentifier identifier = default;

    public override void Initialize(LevelBoundsEntities levelBounds)
    {
        PlayerShipRecord record = identifier.GetRecord();
        Entity prefabEntity = record.GetEntity();
        BuildPhysicsWorld buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity entity = entityManager.Instantiate(record.GetEntity());
        System.Random random = new System.Random(seed);

        SceneIdentifier sceneIdentifier = SceneLoader.instance.currentSceneIdentifier;
        float3 rootPosition = float3.zero;
        quaternion rootRotation = quaternion.identity;

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

        entityManager.SetName(entity, " Player Ship");

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
}