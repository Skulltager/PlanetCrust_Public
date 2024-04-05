
using Unity.Entities;

public abstract partial class ReferenceUpdateSystemBase : SystemBase
{
    private EntityEvent_LateFixedUpdate lateFixedUpdate;

    protected override void OnCreate()
    {
        lateFixedUpdate = World.GetOrCreateSystem<EntityEvent_LateFixedUpdate>();
        lateFixedUpdate.onLateFixedUpdate += OnUpdate;
    }

    protected override void OnDestroy()
    {
        lateFixedUpdate.onLateFixedUpdate -= OnUpdate;
    }
}