
using Unity.Collections;
using Unity.Entities;

[GenerateAuthoringComponent]
public struct EntityColliderReference : IComponentData
{
    [ReadOnly] public ColliderReferenceType referenceType;
}