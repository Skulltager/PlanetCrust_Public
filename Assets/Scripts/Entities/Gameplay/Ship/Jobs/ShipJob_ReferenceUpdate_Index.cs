
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_ReferenceUpdate_Index : SystemBase
{
    protected override void OnUpdate()
    {
        new ShipJob_ReferenceUpdate_Index()
        {
        }.Run();
    }

    [WithAll(typeof(ShipEntityTag))]
    private partial struct ShipJob_ReferenceUpdate_Index : IJobEntity
    {
        private void Execute(
            ShipEntityReference reference,
            ref ShipEntityReferenceIndex referenceIndex)
        {
            referenceIndex.value = reference.value.index;
        }
    }
}