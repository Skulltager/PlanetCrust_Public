
using Unity.Entities;
using Unity.Mathematics;

public struct ShipEntityData_Health : IComponentData
{
    public int maxArmor;
    public int maxHull;
    public int maxShield;
    public int armor;
    public int hull;
    public int shield;
}