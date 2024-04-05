
using UnityEngine;

public static class Vector3Extention
{
    public static Vector3 RotateRound(this Vector3 position, Vector3 axis, float angle)
    {
        return Quaternion.AngleAxis(angle, axis) * position;
    }
}