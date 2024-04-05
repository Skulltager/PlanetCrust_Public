
using Unity.Entities;
using Unity.Mathematics;

public struct ShipEntityData_Player_Energy : IComponentData
{
    public float energyRegeneration;
    public int maxEnergy;
    public float energy;
}