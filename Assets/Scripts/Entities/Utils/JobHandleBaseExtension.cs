
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

public static class JobHandleExtension
{
    public static void AddBuildPhysicsWorldDependencyToComplete(this JobHandle handle)
    {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>().AddInputDependencyToComplete(handle);
    }
}