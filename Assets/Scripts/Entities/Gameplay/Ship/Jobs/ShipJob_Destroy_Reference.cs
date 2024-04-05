
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_Destroy_Reference : SystemBase
{
    protected override void OnUpdate()
    {
        new ShipJob_Destroy_Reference
        {
        }.Run();
    }

    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_Destroy))]
    private partial struct ShipJob_Destroy_Reference : IJobEntity
    {
        private void Execute(
            ShipEntityReference reference)
        {
            reference.value.Destroy();
        }
    }
}
