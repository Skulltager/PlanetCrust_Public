
using Unity.Entities;

public class ShipData_Energy
{
    public EntityManager entityManager => World.DefaultGameObjectInjectionWorld.EntityManager;

    public readonly EventVariable<ShipData_Energy, int> maxEnergy;
    public readonly EventVariable<ShipData_Energy, float> energy;
    public readonly EventVariable<ShipData_Energy, float> energyRegeneration;

    public ShipData_Energy()
    {
        maxEnergy = new EventVariable<ShipData_Energy, int>(this, 100);
        energy = new EventVariable<ShipData_Energy, float>(this, 100);
        energyRegeneration = new EventVariable<ShipData_Energy, float>(this, 10);
    }
}