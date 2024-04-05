
using FMOD;
using System.Net;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PrepareSystemGroup))]
[UpdateBefore(typeof(ShipSystem_AI_SetTargetPosition))]
public partial class ShipSystem_AI_FindNewTarget : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_AI_FindNewTarget
        {
            allShipEntities = ShipData.allShipEntities,
            ref_Team = GetComponentDataFromEntity<ShipEntityData_Team>(true),
            ref_Translation = GetComponentDataFromEntity<Translation>(true),
        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_FindNewTarget : IJobEntity
    {
        [ReadOnly] public NativeArray<Entity> allShipEntities;
        [ReadOnly] public ComponentDataFromEntity<ShipEntityData_Team> ref_Team;
        [ReadOnly] public ComponentDataFromEntity<Translation> ref_Translation;

        private void Execute(
            ref ShipEntityData_AI_Target target,
            in ShipEntityData_Team team,
            in Translation translation)
        {
            // Target still alive
            if (ref_Translation.HasComponent(target.value))
                return;

            float closestDistanceSquared = float.MaxValue;
            Entity closestEntity = default;
            for (int i = 0; i < allShipEntities.Length; i++)
            {
                Entity shipEntity = allShipEntities[i];
                if (!ref_Team.TryGetComponent(shipEntity, out ShipEntityData_Team otherTeam))
                    continue;

                if (team.value == otherTeam.value)
                    continue;

                Translation otherTranslation = ref_Translation[shipEntity];
                float distanceSquared = math.lengthsq(otherTranslation.Value - translation.Value);
                if (distanceSquared > closestDistanceSquared)
                    continue;

                closestDistanceSquared = distanceSquared;
                closestEntity = shipEntity;
            }

            target.value = closestEntity;
        }
    }
}
