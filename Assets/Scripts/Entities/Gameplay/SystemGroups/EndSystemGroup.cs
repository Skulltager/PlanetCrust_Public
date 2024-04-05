
using Unity.Entities;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(DestroySystemGroup))]
public class EndSystemGroup : ComponentSystemGroup
{

}