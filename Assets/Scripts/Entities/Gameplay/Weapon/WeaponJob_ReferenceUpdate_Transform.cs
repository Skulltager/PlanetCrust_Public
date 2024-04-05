
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateInGroup(typeof(EndSystemGroup))]
public partial class WeaponSystem_ReferenceUpdate_Transform : SystemBase
{
    protected override void OnUpdate()
    {
        new WeaponJob_ReferenceUpdate_Transform()
        {
            ref_LocalToWorld = GetComponentDataFromEntity<LocalToWorld>(),
        }.Run();
    }

    [WithAll(typeof(WeaponEntityTag))]
    private partial struct WeaponJob_ReferenceUpdate_Transform : IJobEntity
    {
        [ReadOnly] public ComponentDataFromEntity<LocalToWorld> ref_LocalToWorld;
        private void Execute(
            WeaponEntityReference reference,
            in DynamicBuffer<BufferElement_Reference_WeaponFireLocation> weaponFireLocations)
        {
            for (int i = 0; i < weaponFireLocations.Length; i++)
            {
                LocalToWorld localToWorld = ref_LocalToWorld[weaponFireLocations[i].entity];
                reference.value.transformDatas[i].position.value = localToWorld.Position;
                reference.value.transformDatas[i].rotation.value = localToWorld.Rotation;
            }
        }
    }
}