
using Unity.Entities;

public class ShipData_Health
{
    public EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public readonly EventVariable<ShipData_Health, int> maxHull;
    public readonly EventVariable<ShipData_Health, int> maxShield;
    public readonly EventVariable<ShipData_Health, int> maxArmor;
    public readonly EventVariable<ShipData_Health, int> hull;
    public readonly EventVariable<ShipData_Health, int> shield;
    public readonly EventVariable<ShipData_Health, int> armor;

    public ShipData_Health()
    {
        maxHull = new EventVariable<ShipData_Health, int>(this, 0);
        hull = new EventVariable<ShipData_Health, int>(this, 0);
        maxShield = new EventVariable<ShipData_Health, int>(this, 0);
        shield = new EventVariable<ShipData_Health, int>(this, 0);
        maxArmor = new EventVariable<ShipData_Health, int>(this, 0);
        armor = new EventVariable<ShipData_Health, int>(this, 0);
    }
}