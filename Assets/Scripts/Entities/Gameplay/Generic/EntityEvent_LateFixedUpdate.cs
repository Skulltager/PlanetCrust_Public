
using System;
using Unity.Entities;

[UpdateInGroup(typeof(EndSystemGroup), OrderLast = true)]
public partial class EntityEvent_LateFixedUpdate : SystemBase
{
    public event Action onLateFixedUpdate;

    protected override void OnUpdate()
    {
        if (onLateFixedUpdate != null)
            onLateFixedUpdate();
    }
}