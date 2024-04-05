
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class ShipSystem_ReferenceUpdate_Health : SystemBase
{
    protected override void OnUpdate()
    {
        new ShipJob_ReferenceUpdate_Health()
        {
        }.Run();
    }

    [WithAll(typeof(ShipEntityTag))]
    private partial struct ShipJob_ReferenceUpdate_Health : IJobEntity
    {
        private void Execute(
            ShipEntityReference reference,
            in ShipEntityData_Health health)
        {
            reference.value.healthData.armor.value = health.armor;
            reference.value.healthData.shield.value = health.shield;
            reference.value.healthData.hull.value = health.hull;
            reference.value.healthData.maxArmor.value = health.maxArmor;
            reference.value.healthData.maxShield.value = health.maxShield;
            reference.value.healthData.maxHull.value = health.maxHull;
        }
    }
}