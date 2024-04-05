
using Unity.Entities;

[UpdateInGroup(typeof(InitializeSystemGroup))]
[UpdateAfter(typeof(WeaponSystem_Initialize_Reference))]
public partial class WeaponSystem_Initialize_Fire_Reference : SystemBase
{
    protected override void OnUpdate()
    {
        new WeaponJob_Initialize_Fire_Reference()
        {
        }.Run();
    }

    [WithAll(typeof(WeaponEntityTag), typeof(EntityTag_Initialize))]
    private partial struct WeaponJob_Initialize_Fire_Reference : IJobEntity
    {
        private void Execute(
            WeaponEntityReference reference,
            in WeaponEntityData_Fire fire)
        {
            reference.value.fireData.firingAngle.value = fire.firingAngle;
            reference.value.fireData.spread.value = fire.spread;
        }
    }
}