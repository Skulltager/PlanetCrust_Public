
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_ReferenceUpdate_Energy : SystemBase
{
    protected override void OnUpdate()
    {
        new ShipJob_ReferenceUpdate_Energy()
        {
        }.Run();
    }

    [WithAll(typeof(ShipEntityTag))]
    private partial struct ShipJob_ReferenceUpdate_Energy : IJobEntity
    {
        private void Execute(
            ShipEntityReference reference,
            in ShipEntityData_Player_Energy energy)
        {
            reference.value.energyData.maxEnergy.value = energy.maxEnergy;
            reference.value.energyData.energy.value = energy.energy;
            reference.value.energyData.energyRegeneration.value = energy.energyRegeneration;
        }
    }
}