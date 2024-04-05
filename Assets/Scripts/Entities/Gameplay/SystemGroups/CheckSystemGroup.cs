
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PostUpdateSystemGroup))]
public class CheckSystemGroup : ComponentSystemGroup
{

}