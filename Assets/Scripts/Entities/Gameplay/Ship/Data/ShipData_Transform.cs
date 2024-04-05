using UnityEngine;

public class ShipData_Transform
{
    public readonly EventVariable<ShipData_Transform, Vector3> position;
    public readonly EventVariable<ShipData_Transform, Quaternion> rotation;

    public ShipData_Transform()
    {
        position = new EventVariable<ShipData_Transform, Vector3>(this, default);
        rotation = new EventVariable<ShipData_Transform, Quaternion>(this, default);
    }
}