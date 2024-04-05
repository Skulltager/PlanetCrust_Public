
using Unity.Entities;

public struct ProjectileEntityData_Damage : IComponentData
{
    public int minDamage;
    public int maxDamage;
    public DamageType damageType;
}