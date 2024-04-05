
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateInGroup(typeof(PostUpdateSystemGroup))]
[UpdateBefore(typeof(WeaponSystem_Player_Fire))]
public partial class WeaponSystem_Player_ActivatePrimary : SystemBase
{

    protected override void OnUpdate()
    {
        JobHandle handle = new WeaponJob_Player_ActivatePrimary
        {
            activate = ControlManager.instance.IsFiringPrimaryWeapon(),
        }.ScheduleParallel();
    }

    [BurstCompile]
    [WithAll(typeof(WeaponEntityTag), typeof(EntityTag_PlayerControlled))]
    private partial struct WeaponJob_Player_ActivatePrimary : IJobEntity
    {
        public bool activate;

        private void Execute(
            [EntityInQueryIndex] int entityIndex,
            ref WeaponEntityData_Active active)
        {
            active.value = activate;
        }
    }
}