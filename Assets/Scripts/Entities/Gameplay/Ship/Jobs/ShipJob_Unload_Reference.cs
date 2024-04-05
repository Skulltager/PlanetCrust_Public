using Unity.Entities;

[WithAll(typeof(ShipEntityTag))]
public partial struct ShipJob_Unload_Reference : IJobEntity
{
    private void Execute(
        ShipEntityReference reference)
    {
        reference.value.Destroy();
    }
}