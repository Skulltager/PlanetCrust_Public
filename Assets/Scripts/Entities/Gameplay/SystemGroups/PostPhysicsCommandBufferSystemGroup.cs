
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(CollisionEventSystemGroup))]
public class PostPhysicsCommandBufferSystemGroup : ComponentSystemGroup
{

}