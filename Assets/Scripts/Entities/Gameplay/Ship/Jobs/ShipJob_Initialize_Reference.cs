
using Unity.Entities;

[UpdateInGroup(typeof(InitializeSystemGroup))]
public partial class ShipSystem_Initialize_Reference : SystemBase
{
    protected override void OnCreate()
    {
        ShipData.Initialize();
    }

    protected override void OnUpdate()
    {
        new ShipJob_Initialize_Reference()
        {
        }.Run();
    }

    protected override void OnDestroy()
    {
        ShipData.allShipEntities.Dispose();
    }

    [WithAll(typeof(ShipEntityTag), typeof(EntityTag_Initialize))]
    private partial struct ShipJob_Initialize_Reference : IJobEntity
    {
        private void Execute(
            Entity entity,
            ShipEntityReference reference)
        {
            reference.value = new ShipData(entity);
        }
    }

}