
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PrepareSystemGroup))]
public partial class ShipSystem_AI_SetTargetPosition : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_AI_SetTargetPosition
        {
            ref_Translation = GetComponentDataFromEntity<Translation>(true),

        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_AIControlled))]
    private partial struct ShipJob_AI_SetTargetPosition : IJobEntity
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> ref_Translation;

        private void Execute(
            ref ShipEntityData_TargetPosition targetPosition,
            in ShipEntityData_AI_Target target)
        {
            if (!ref_Translation.TryGetComponent(target.value, out Translation translation))
                return;

            targetPosition.targetPosition = translation.Value;
        }
    }
}