
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PreUpdateSystemGroup))]
public partial class ShipSystem_Energy_Regeneration : SystemBase
{
    protected override void OnUpdate()
    {
        JobHandle handle = new ShipJob_Energy_Regeneration()
        {
            deltaTime = Time.DeltaTime,
        }.ScheduleParallel();
    }
    
    [BurstCompile]
    private partial struct ShipJob_Energy_Regeneration : IJobEntity
    {
        public float deltaTime;
        private void Execute(
            ref ShipEntityData_Player_Energy energy)
        {
            energy.energy = math.min(energy.energy + energy.energyRegeneration * deltaTime, energy.maxEnergy);
        }
    }

}
