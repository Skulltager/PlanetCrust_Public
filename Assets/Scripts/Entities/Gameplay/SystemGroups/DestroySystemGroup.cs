
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(CheckSystemGroup))]
public class DestroySystemGroup : ComponentSystemGroup
{

}