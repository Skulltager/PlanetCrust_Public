
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

[UpdateInGroup(typeof(InitializeSystemGroup))]
public partial class EntitySystem_Initialize_Random : SystemBase
{
    private uint totalEntityCount;
    private EntityQuery entityQuery;

    protected override void OnCreate()
    {
        entityQuery = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<EntityTag_Initialize>());
    }

    protected override void OnUpdate()
    {
        uint entityCount = (uint)entityQuery.CalculateEntityCount();

        JobHandle handle = new EntityJob_Initialize_Random
        {
            totalEntityCount = totalEntityCount,
        }.ScheduleParallel();

        totalEntityCount += entityCount;
    }
}

[BurstCompile]
[WithAll(typeof(EntityTag_Initialize))]
public partial struct EntityJob_Initialize_Random : IJobEntity
{
    public uint totalEntityCount;

    public void Execute(
        [EntityInQueryIndex] int entityIndex,
        ref EntityData_Random random)
    {
        random.value = Random.CreateFromIndex(totalEntityCount + (uint)entityIndex);
    }
}