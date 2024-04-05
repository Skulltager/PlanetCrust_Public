
using Unity.Entities;

public struct EntityData_ImpactDamage : IComponentData
{
    public int baseMinImpactDamage;
    public int baseMaxImpactDamage;
    public float minVelocityImpactDamage;
    public float maxVelocityImpactDamage;
    public float minVelocityImpactSpeed;
}