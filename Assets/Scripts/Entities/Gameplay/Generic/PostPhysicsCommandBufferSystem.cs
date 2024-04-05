
using Unity.Entities;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PostPhysicsCommandBufferSystemGroup))]

[AlwaysSynchronizeSystem]
public class PostPhysicsCommandBufferSystem : EntityCommandBufferSystem
{

}