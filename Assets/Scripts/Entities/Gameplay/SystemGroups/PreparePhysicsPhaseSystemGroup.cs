
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(InitializeSystemGroup))]
public class PreparePhysicsPhaseSystemGroup : ComponentSystemGroup
{

}