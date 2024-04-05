
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_ReferenceUpdate_Transform : SystemBase
{
    protected override void OnUpdate()
    {
        new ShipJob_ReferenceUpdate_Transform()
        {
        }.Run();
    }

    [WithAll(typeof(ShipEntityTag))]
    private partial struct ShipJob_ReferenceUpdate_Transform : IJobEntity
    {
        private void Execute(
            ShipEntityReference reference,
            in Translation translation,
            in Rotation rotation)
        {
            reference.value.transformData.position.value = translation.Value;
            reference.value.transformData.rotation.value = rotation.Value;
        }
    }
}