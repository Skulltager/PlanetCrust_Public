
using Unity.Entities;
using UnityEngine;
using SheetCodes;
using System;
using System.Collections.Generic;

public class SheetCodesEntityPrefabReferencer : MonoBehaviour
{
    public static Dictionary<ProjectileIdentifier, Entity> projectileEntities;
    public static Dictionary<AiShipIdentifier, Entity> aiShipEntities;
    public static Dictionary<PlayerShipIdentifier, Entity> playerShipEntities;
    private static List<BlobAssetStore> blobAssetStores;

    private void Awake()
    {
        projectileEntities = new Dictionary<ProjectileIdentifier, Entity>();
        aiShipEntities = new Dictionary<AiShipIdentifier, Entity>();
        playerShipEntities = new Dictionary<PlayerShipIdentifier, Entity>();
        blobAssetStores = new List<BlobAssetStore>();

        ProjectileIdentifier[] projectileIdentifiers = Enum.GetValues(typeof(ProjectileIdentifier)) as ProjectileIdentifier[];
        for (int i = 0; i < projectileIdentifiers.Length; i++)
        {
            ProjectileIdentifier identifier = projectileIdentifiers[i];
            if (identifier == ProjectileIdentifier.None)
                continue;

            ProjectileRecord record = identifier.GetRecord();

            BlobAssetStore blobAssetStore = new BlobAssetStore();
            blobAssetStores.Add(blobAssetStore);
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
            projectileEntities.Add(identifier, GameObjectConversionUtility.ConvertGameObjectHierarchy(record.Prefab, settings));
        }

        AiShipIdentifier[] aiShipIdentifiers = Enum.GetValues(typeof(AiShipIdentifier)) as AiShipIdentifier[];
        for (int i = 0; i < aiShipIdentifiers.Length; i++)
        {
            AiShipIdentifier identifier = aiShipIdentifiers[i];
            if (identifier == AiShipIdentifier.None)
                continue;

            AiShipRecord record = identifier.GetRecord();

            BlobAssetStore blobAssetStore = new BlobAssetStore();
            blobAssetStores.Add(blobAssetStore);
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
            aiShipEntities.Add(identifier, GameObjectConversionUtility.ConvertGameObjectHierarchy(record.Prefab, settings));
        }

        PlayerShipIdentifier[] playerShipIdentifiers = Enum.GetValues(typeof(PlayerShipIdentifier)) as PlayerShipIdentifier[];
        for (int i = 0; i < aiShipIdentifiers.Length; i++)
        {
            PlayerShipIdentifier identifier = playerShipIdentifiers[i];
            if (identifier == PlayerShipIdentifier.None)
                continue;

            PlayerShipRecord record = identifier.GetRecord();

            BlobAssetStore blobAssetStore = new BlobAssetStore();
            blobAssetStores.Add(blobAssetStore);
            GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
            playerShipEntities.Add(identifier, GameObjectConversionUtility.ConvertGameObjectHierarchy(record.Prefab, settings));
        }
    }

    private void OnDestroy()
    {
        foreach (BlobAssetStore blobAssetStore in blobAssetStores)
            blobAssetStore.Dispose();
    }
}

public static class SheetCodesEntityExtention
{
    public static Entity GetEntity(this PlayerShipRecord record)
    {
        return SheetCodesEntityPrefabReferencer.playerShipEntities[record.Identifier];
    }

    public static Entity GetEntity(this AiShipRecord record)
    {
        return SheetCodesEntityPrefabReferencer.aiShipEntities[record.Identifier];
    }

    public static Entity GetEntity(this ProjectileRecord record)
    {
        return SheetCodesEntityPrefabReferencer.projectileEntities[record.Identifier];
    }
}