
using Unity.Entities;
using Unity.Mathematics;

public struct ShipEntityData_TakeDamage : IComponentData
{
    public int amount;
    public DamageType damageType;
    public Entity ship;
}