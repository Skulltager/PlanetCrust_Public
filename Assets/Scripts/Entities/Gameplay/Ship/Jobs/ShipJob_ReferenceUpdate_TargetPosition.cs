
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_ReferenceUpdate_TargetPosition : SystemBase
{
    protected override void OnUpdate()
    {
        new ShipJob_ReferenceUpdate_TargetPosition()
        {
        }.Run();
    }

    [WithAll(typeof(ShipEntityTag), typeof(ShipEntityTag_TrackTargetPosition))]
    private partial struct ShipJob_ReferenceUpdate_TargetPosition : IJobEntity
    {
        private void Execute(
            ShipEntityReference reference,
            ref ShipEntityData_TargetPosition targetPosition)
        {
            reference.value.targetPositionData.targetPosition.value = targetPosition.targetPosition;
        }
    }
}