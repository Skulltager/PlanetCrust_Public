using UnityEngine;

public class ShipData_TargetPosition
{
    public readonly EventVariable<ShipData_TargetPosition, Vector3> targetPosition;

    public ShipData_TargetPosition()
    {
        targetPosition = new EventVariable<ShipData_TargetPosition, Vector3>(this, default);
    }
}