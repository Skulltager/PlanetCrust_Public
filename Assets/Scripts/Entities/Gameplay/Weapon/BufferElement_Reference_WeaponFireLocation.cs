
using Unity.Entities;

[InternalBufferCapacity(5)]
public struct BufferElement_Reference_WeaponFireLocation : IBufferElementData
{
    public Entity entity;

    public static implicit operator Entity(BufferElement_Reference_WeaponFireLocation item) 
    { 
        return item.entity; 
    }

    public static implicit operator BufferElement_Reference_WeaponFireLocation(Entity item) 
    { 
        return new BufferElement_Reference_WeaponFireLocation { entity = item }; 
    }
}