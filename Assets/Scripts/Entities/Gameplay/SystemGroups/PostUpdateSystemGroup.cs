
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PostPhysicsCommandBufferSystemGroup))]
public class PostUpdateSystemGroup : ComponentSystemGroup
{

}