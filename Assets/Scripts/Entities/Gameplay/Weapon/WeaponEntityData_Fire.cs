
using Unity.Entities;

public struct WeaponEntityData_Fire : IComponentData
{
    public Entity projectilePrefab;
    public float projectileSpeed;
    public double cooldown;
    public float spread;
    public float firingAngle;
    public int projectileCount;
}