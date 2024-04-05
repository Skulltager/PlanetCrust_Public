
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PreUpdateSystemGroup))]
public class PrepareSystemGroup : ComponentSystemGroup
{

}