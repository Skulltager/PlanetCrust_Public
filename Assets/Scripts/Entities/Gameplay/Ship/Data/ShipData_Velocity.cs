
using UnityEngine;

public class ShipData_Velocity
{
    public readonly EventVariable<ShipData_Velocity, Vector3> velocity;
    public readonly EventVariable<ShipData_Velocity, Vector3> angularVelocity;

    public ShipData_Velocity()
    {
        velocity = new EventVariable<ShipData_Velocity, Vector3>(this, default);
        angularVelocity = new EventVariable<ShipData_Velocity, Vector3>(this, default);
    }
}