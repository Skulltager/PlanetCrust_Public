
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PrepareSystemGroup))]
public class InitializeSystemGroup : ComponentSystemGroup
{

}